﻿namespace Low_Code_App_Editor_1.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;

	using Low_Code_App_Editor_1;
	using Low_Code_App_Editor_1.DOM;
	using Low_Code_App_Editor_1.LCA;
	using Low_Code_App_Editor_1.Package;
	using Low_Code_App_Editor_1.UI;
	using Low_Code_App_Editor_1.Json;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.AppPackages;
	using Skyline.DataMiner.Net.Apps.FileTime;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Web.Common.v1.Dashboards;
	using Newtonsoft.Json.Linq;

	public class ExportController
	{
		public static readonly string ScriptPath = @"C:\Skyline DataMiner\Scripts";
		public static readonly string DllImportPath = @"C:\Skyline DataMiner\ProtocolScripts\DllImport";
		public static readonly string LowCodeAppEditorPath = @"C:\Skyline DataMiner\Documents\Low Code App Editor";
		public static readonly string LowCodeAppEditorExportPath = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\Low Code Apps Exports";
		public static readonly string ThemesPath = @"C:\Skyline DataMiner\dashboards\Themes.json";

		public static string ExportApps(IEngine engine, IEnumerable<App> apps, ExportOptions options)
		{
			return ExportPackage(engine, apps, options);
		}

		private static string ExportPackage(IEngine engine, IEnumerable<App> apps, ExportOptions options)
		{
			var now = DateTime.Now;

			var scriptReferences = new Dictionary<string, List<string>>();
			var exportPath = Path.Combine(LowCodeAppEditorExportPath, $"{now.ToString("yyyy-MM-dd HH-mm-ss")}_App_Export.zip");
			if (apps.Count() == 1)
			{
				// When only one app is selected we can include the app name in the package name
				exportPath = Path.Combine(LowCodeAppEditorExportPath, $"{apps.First().Name}_{now.ToString("yyyy-MM-dd HH-mm-ss")}_App_Export.zip");
			}

			engine.GenerateInformation($"Export Path: {exportPath}");

			if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
			}

			using (var fs = new FileStream(exportPath, FileMode.Create))
			using (var zip = new ZipArchive(fs, ZipArchiveMode.Create))
			{
				engine.GenerateInformation($"Adding Package Information");

				// Package Information
				var info = apps.Count() == 1 ? PackageInfo.FromApp(apps.First()) : PackageInfo.FromApp();
				zip.CreateEntryFromText("AppInfo.xml", XmlConvert.SerializeObject(info));
				zip.CreateEntryFromText(Path.Combine("AppInstallContent", "DeploymentActions.xml"), "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<AppPackageDeploymentActions>\r\n  <DefaultProtocolVisios />\r\n  <ProductionProtocols />\r\n  <TemplatesToPreserve>\r\n    <AlarmTemplates />\r\n    <InformationTemplates />\r\n    <TrendTemplates />\r\n  </TemplatesToPreserve>\r\n</AppPackageDeploymentActions>");

				// Package install options
				zip.CreateEntryFromText(Path.Combine("AppInstallContent", "InstallOptions.json"), JsonConvert.SerializeObject(new InstallOptions
				{
					OverwritePreviousVersions = options.OverwritePreviousVersions,
				}));

				var domModuleIds = new List<string>();
				var images = new List<string>();
				var themes = new List<DMADashboardTheme>();
				foreach (var app in apps)
				{
					engine.GenerateInformation($"Adding App");

					// Add the app as CompanionFiles
					AddAppToArchive(zip, app, options);

					if (options.ExcludeScripts)
					{
						engine.GenerateInformation("Skipping Automation Scripts");
					}
					else
					{
						engine.GenerateInformation($"Adding Scripts");

						// Add the scripts used in the App
						var scripts = AddScriptsToArchive(zip, app);

						engine.GenerateInformation($"Adding Script dependencies");

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
					}

					if (!options.ExcludeDom)
					{
						// Add Dom definitions
						domModuleIds.AddRangeUnique(app.LatestVersion.GetUsedDomModules());
					}

					if (!options.ExcludeImages)
					{
						// Add Images to companion files
						images.AddRangeUnique(app.LatestVersion.GetUsedImages());
					}

					if (!options.ExcludeThemes)
					{
						// Add Theme
						themes.AddRangeUnique(app.LatestVersion.GetUsedThemes());
					}
				}

				// Add DOM modules
				if (options.ExcludeDom)
				{
					engine.GenerateInformation($"Skipping DOM modules");
				}
				else
				{
					engine.GenerateInformation($"Adding DOM modules");
					AddDomToArchive(engine, zip, domModuleIds, options);
				}

				if (options.ExcludeImages)
				{
					engine.GenerateInformation($"Skipping Images");
				}
				else
				{
					// Add Images
					engine.GenerateInformation($"Adding Images");
					AddImagesToArchive(zip, images);
				}

				if (options.ExcludeThemes)
				{
					engine.GenerateInformation($"Skipping Themes");
				}
				else
				{
					// Add Themes
					engine.GenerateInformation($"Adding Themes");
					AddThemesToArchive(zip, themes);
				}

				engine.GenerateInformation($"Adding Installer code");

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
				if (app.LatestDraftVersion != null)
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
				var scriptPath = Path.Combine(ScriptPath, scriptName);
				if (File.Exists(scriptPath))
				{
					zip.CreateEntryFromFile(Path.Combine(ScriptPath, scriptName), Path.Combine("AppInstallContent", "Scripts", script, scriptName));
				}
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
				var scriptPath = Path.Combine(ScriptPath, scriptName);
				if (!File.Exists(scriptPath))
				{
					continue;
				}

				var scriptFile = System.IO.File.ReadAllText(scriptPath);
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

		private static void AddImagesToArchive(ZipArchive zip, List<string> images)
		{
			var imageFolder = Path.Combine("AppInstallContent", "CompanionFiles", "Images") + "\\"; // Slash to indicate it's a directory
			zip.CreateEntry(imageFolder);

			foreach (var image in images)
			{
				zip.CreateEntryFromFile(image, Path.Combine("AppInstallContent", "CompanionFiles", "Images", Path.GetFileName(image)));
			}
		}

		private static void AddThemesToArchive(ZipArchive zip, List<DMADashboardTheme> themes)
		{
			// Original themes
			var themesJson = JObject.Parse(File.ReadAllText(ThemesPath));
			var allThemes = themesJson["Themes"] as JArray;

			// Create a copy of the themes object, clear all the themes out.
			var usedThemesJson = themesJson.DeepClone();
			var usedThemesArray = usedThemesJson["Themes"] as JArray;
			usedThemesArray.Clear();

			// Add the ones needed for the package to the cloned
			foreach (var theme in themes)
			{
				var usedTheme = allThemes.First(t => t["Name"].Value<string>() == theme.Name);
				usedThemesArray.Add(usedTheme);
			}

			var themesPath = Path.Combine("AppInstallContent", "CompanionFiles", "Themes") + "\\"; // Slash to indicate it's a directory
			zip.CreateEntry(themesPath);
			zip.CreateEntryFromText(Path.Combine(themesPath, "Themes.json"), usedThemesJson.ToString());
		}
	}

	public class ExportOptions
	{
		public bool IncludeVersions { get; set; }

		public bool ExportPackage { get; } = true;

		public bool OverwritePreviousVersions { get; set; }

		public bool ExcludeScripts { get; set; }

		public bool ExcludeDom { get; set; }

		public bool ExportDomInstances { get; set; }

		public bool ExcludeImages { get; set; }

		public bool ExcludeThemes { get; set; }

		public static ExportOptions FromDialog(ExportDialog dialog)
		{
			return new ExportOptions
			{
				IncludeVersions = dialog.ExportVersions.IsChecked,
				OverwritePreviousVersions = dialog.OverwritePreviousVersions.IsChecked,
				ExcludeScripts = dialog.ExcludeScripts.IsChecked,
				ExcludeDom = dialog.ExcludeDom.IsChecked,
				ExportDomInstances = dialog.ExportDomInstances.IsChecked,
				ExcludeImages = dialog.ExcludeImages.IsChecked,
				ExcludeThemes = dialog.ExcludeThemes.IsChecked,
			};
		}
	}
}
