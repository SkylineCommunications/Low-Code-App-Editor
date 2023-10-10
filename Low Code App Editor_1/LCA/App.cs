namespace Low_Code_App_Editor_1.LCA
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Low_Code_App_Editor_1.Json;
    using Newtonsoft.Json;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Web.Common.v1;

    public class App
    {
        public App(string path, IEngine engine)
        {
            Path = path;

            // Load general settings file
            var settingsFile = File.ReadAllText(System.IO.Path.Combine(path, "App.info.json"));
            Settings = JsonConvert.DeserializeObject<DMAApplicationVersionInfo>(settingsFile);

            // Load version history
            var versions = new string[0];
            if (Directory.Exists(path))
            {
                versions = Directory.GetDirectories(path);
            }

            // Load latest public version
            if(Settings.PublicVersion > 0)
            {
                var versionDirectory = versions.FirstOrDefault(x => x.Contains($"version_{Settings.PublicVersion}"));
                var versionPath = System.IO.Path.Combine(versionDirectory, "App.config.json");
                if (File.Exists(versionPath))
                {
                    var version = JsonConvert.DeserializeObject<AppVersion>(File.ReadAllText(versionPath), new TypeConverter());
                    version.Path = versionPath;
                    Versions.Add(version);
                }
            }

            // Load latest draft version
            if(Settings.DraftVersion > 0)
            {
                var versionDirectory = versions.FirstOrDefault(x => x.Contains($"version_{Settings.DraftVersion}"));
                var versionPath = System.IO.Path.Combine(versionDirectory, "App.config.json");
                if (File.Exists(versionPath))
                {
                    var version = JsonConvert.DeserializeObject<AppVersion>(File.ReadAllText(versionPath), new TypeConverter());
                    version.Path = versionPath;
                    Versions.Add(version);
                }
            }
        }

        public string Path { get; }

        public string PathSettings { get => System.IO.Path.Combine(Path, "App.info.json"); }

        public List<AppVersion> Versions { get; } = new List<AppVersion>();

        public DMAApplicationVersionInfo Settings { get; }

        public string Name
        {
            get
            {
                if(LatestVersion != null)
                {
                    return LatestVersion.Name;
                }
                else if(LatestDraftVersion != null)
                {
                    return LatestDraftVersion.Name;
                }
                else
                {
                    throw new NullReferenceException("No Draft or Public version found.");
                }
            }
        }

        public string Description
        {
            get
            {
                if (LatestVersion != null)
                {
                    return LatestVersion.Description;
                }
                else if (LatestDraftVersion != null)
                {
                    return LatestDraftVersion.Description;
                }
                else
                {
                    throw new NullReferenceException("No Draft or Public version found.");
                }
            }
        }

        public AppVersion LatestVersion
        {
            get
            {
                var latestRelease = Versions.FirstOrDefault(version => version.Version == Settings.PublicVersion);
                if(latestRelease == null)
                {
                    return LatestDraftVersion;
                }

                return latestRelease;
            }
        }

        public AppVersion LatestDraftVersion
        {
            get => Versions.FirstOrDefault(version => version.Version == Settings.DraftVersion);
        }
    }
}
