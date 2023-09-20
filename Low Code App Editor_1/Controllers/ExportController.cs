namespace Low_Code_App_Editor_1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Low_Code_App_Editor_1.DOM;
    using Low_Code_App_Editor_1.LCA;
    using Low_Code_App_Editor_1.Package;
    using Low_Code_App_Editor_1.UI;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.AppPackages;
    using Skyline.DataMiner.Net.Apps.Modules;

    public class ExportController
    {
        public static readonly string ScriptPath = @"C:\Skyline DataMiner\Scripts";
        public static readonly string DllImportPath = @"C:\Skyline DataMiner\ProtocolScripts\DllImport";
        public static readonly string LowCodeAppEditorPath = @"C:\Skyline DataMiner\Documents\Low Code App Editor";
        public static readonly string LowCodeAppEditorExportPath = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\Low Code Apps Exports";

        public static string ExportApps(IEngine engine, IEnumerable<App> apps, ExportOptions options)
        {
            if(options.ExportPackage)
            {
                return ExportPackage(engine, apps, options);
            }
            else
            {
                return ExportBasic(apps, options);
            }
        }

        private static string ExportBasic(IEnumerable<App> apps, ExportOptions options)
        {
            var now = DateTime.Now;
            var exportPath = Path.Combine(LowCodeAppEditorExportPath, $"{now.ToString("yyyy-MM-dd HH-mm-ss")}_App_Export.zip");

            if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            }

            using (var fs = new FileStream(exportPath, FileMode.Create))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (var app in apps)
                {
                    if (!options.IncludeVersions)
                    {
                        // Just include the general .json file and the latest version
                        zip.CreateEntryFromDirectory(app.Path, null, false);
                        zip.CreateEntryFromDirectory(Path.Combine(app.Path, $"version_{app.LatestVersion.Version} "), Path.Combine(app.LatestVersion.ID, $"version_{app.LatestVersion.Version}"), true);
                    }
                    else
                    {
                        // Include everything
                        zip.CreateEntryFromDirectory(app.Path);
                    }
                }
            }

            return exportPath;
        }

        private static string ExportPackage(IEngine engine, IEnumerable<App> apps, ExportOptions options)
        {
            var now = DateTime.Now;
            var exportPath = Path.Combine(LowCodeAppEditorExportPath, $"{now.ToString("yyyy-MM-dd HH-mm-ss")}_App_Export.zip");
            var scriptReferences = new Dictionary<string, List<string>>();

            if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            }

            using (var fs = new FileStream(exportPath, FileMode.Create))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                // Package Information
                zip.CreateEntryFromText("AppInfo.xml", XmlConvert.SerializeObject(PackageInfo.FromApp()));
                zip.CreateEntryFromText(Path.Combine("AppInstallContent", "DeploymentActions.xml"), "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AppPackageDeploymentActions>\r\n  <DefaultProtocolVisios />\r\n  <ProductionProtocols />\r\n  <TemplatesToPreserve>\r\n    <AlarmTemplates />\r\n    <InformationTemplates />\r\n    <TrendTemplates />\r\n  </TemplatesToPreserve>\r\n</AppPackageDeploymentActions>");

                var domModuleIds = new List<string>();
                foreach(var app in apps)
                {
                    // Add the app as CompanionFiles
                    AddAppToArchive(zip, app, options);

                    // Add the scripts used in the App
                    var scripts = AddScriptsToArchive(zip, app);

                    // Add script dependencies
                    var appReferences = AddDependenciesToArchive(zip, app, scripts);
                    foreach (var pair in appReferences)
                    {
                        if (!scriptReferences.ContainsKey(pair.Key))
                        {
                            scriptReferences.Add(pair.Key, pair.Value);
                        }
                        else
                        {
                            scriptReferences[pair.Key].AddRange(pair.Value);
                        }
                    }

                    // Add Dom definitions
                    domModuleIds.AddRange(app.LatestVersion.GetUsedDomModules());
                }

                // Add DOM modules
                AddDomToArchive(engine, zip, domModuleIds, options);

                // Add custom Low Code App Installer Code
                zip.CreateEntryFromDirectory(LowCodeAppEditorPath, "Scripts");
            }

            var sb = new StringBuilder();
            using (var fs = new FileStream(exportPath, FileMode.Open))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                // Create and Add Description file
                sb.AppendLine($"Low Code App Install Package version: 1.0.1");
                sb.AppendLine($"---------------------------------");
                sb.AppendLine($"Package creation time: {now.ToString("yyyy-MM-dd HH:mm:ss")}");
                sb.AppendLine($"---------------------------------");
                sb.AppendLine($"File Version:");

                // Add scripts
                foreach (var file in zip.GetEntries("AppInstallContent\\Scripts"))
                {
                    sb.AppendLine($"Script\\{Path.GetDirectoryName(file.FullName)}");
                }

                // Add Assemblies
                foreach (var pair in scriptReferences)
                {
                    foreach (var reference in pair.Value)
                    {
                        sb.AppendLine($"Assembly\\{Path.GetDirectoryName(reference)} for automationscript:{pair.Key}");
                    }
                }

                // Add CompanionFiles
                foreach (var file in zip.GetEntries("AppInstallContent\\CompanionFiles"))
                {
                    sb.AppendLine($"CompanionFile\\{file.FullName.Replace("AppInstallContent\\CompanionFiles\\", string.Empty)}");
                }
            }

            using (var fs = new FileStream(exportPath, FileMode.Open))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                zip.CreateEntryFromText("Description.txt", sb.ToString());
            }

            // Rename to .dmapp
            var fileName = exportPath.Replace(".zip", ".dmapp");
            File.Move(exportPath, fileName);
            return fileName;
        }

        private static void AddAppToArchive(ZipArchive zip, App app, ExportOptions options)
        {
            zip = zip ?? throw new ArgumentNullException(nameof(zip));
            app = app ?? throw new ArgumentNullException(nameof(app));
            options = options ?? throw new ArgumentNullException(nameof(options));

            if (!options.IncludeVersions)
            {
                // Just include the general .json file and the latest version
                zip.CreateEntryFromDirectory(app.Path, Path.Combine("AppInstallContent", "CompanionFiles", "LCA", app.LatestVersion.ID), false);
                zip.CreateEntryFromDirectory(Path.Combine(app.Path, $"version_{app.LatestVersion.Version} "), Path.Combine("AppInstallContent", "CompanionFiles", "LCA", app.LatestVersion.ID, $"version_{app.LatestVersion.Version}"), true);
                if(app.LatestDraftVersion != null)
                    zip.CreateEntryFromDirectory(Path.Combine(app.Path, $"version_{app.LatestDraftVersion.Version} "), Path.Combine("AppInstallContent", "CompanionFiles", "LCA", app.LatestDraftVersion.ID, $"version_{app.LatestDraftVersion.Version}"), true);
            }
            else
            {
                // Include everything
                zip.CreateEntryFromDirectory(app.Path, Path.Combine("AppInstallContent", "CompanionFiles", "LCA", app.LatestVersion.ID));
            }
        }

        private static List<string> AddScriptsToArchive(ZipArchive zip, App app)
        {
            var scripts = app.LatestVersion.GetUsedScripts();
            foreach (var script in scripts)
            {
                var scriptName = $"Script_{script}.xml";
                zip.CreateEntryFromFile(Path.Combine(ScriptPath, scriptName), Path.Combine("AppInstallContent", "Scripts", script, scriptName));
            }

            return scripts;
        }

        private static Dictionary<string, List<string>> AddDependenciesToArchive(ZipArchive zip, App app, IEnumerable<string> scripts)
        {
            var scriptReferences = new Dictionary<string, List<string>>();
            foreach (var script in scripts)
            {
                if (!scriptReferences.ContainsKey(script))
                    scriptReferences.Add(script, new List<string>());
                var scriptName = $"Script_{script}.xml";
                var scriptFile = System.IO.File.ReadAllText(Path.Combine(ScriptPath, scriptName));
                XDocument doc = XDocument.Parse(scriptFile);
                XNamespace ns = "http://www.skyline.be/automation";
                var refParams = doc.Descendants(ns + "Param").Where(param => (string)param.Attribute("type") == "ref");
                foreach (var reference in refParams.Select(refParam => refParam.Value))
                {
                    if (reference.StartsWith(DllImportPath))
                    {
                        scriptReferences[script].Add(reference);
                        zip.CreateEntryFromFile(reference, reference.Replace(@"C:\Skyline DataMiner", "AppInstallContent\\Assemblies"));
                    }
                }
            }
            return scriptReferences;
        }

        private static void AddDomToArchive(IEngine engine, ZipArchive zip, IEnumerable<string> domModuleIds, ExportOptions options)
        {
            var moduleSettingsHelper = new ModuleSettingsHelper(engine.SendSLNetMessages);
            var domExporter = new DomExporter(moduleSettingsHelper, engine.SendSLNetMessages);

            var domExport = domExporter.Export(domModuleIds, options.ExportDomInstances);
            zip.CreateEntryFromText(Path.Combine("AppInstallContent", "CompanionFiles", "DOM", "module.json"), domExport);
        }
    }

    public class ExportOptions
    {
        public bool IncludeVersions { get; set; }

        public bool ExportPackage { get; set; }

        public bool ExportDomInstances { get; set; }

        public static ExportOptions FromDialog(ExportDialog dialog)
        {
            return new ExportOptions
            {
                IncludeVersions = dialog.ExportVersions.IsChecked,
                ExportPackage = dialog.ExportPackage.IsChecked,
                ExportDomInstances = dialog.ExportDomInstances.IsChecked,
            };
        }
    }
}
