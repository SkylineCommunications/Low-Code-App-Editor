namespace Low_Code_App_Editor_1.LCA
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Low_Code_App_Editor_1.Json;
    using Newtonsoft.Json;
    using Skyline.DataMiner.Web.Common.v1;

    public class App
    {
        public App(string path)
        {
            Path = path;

            // Load version history
            var versions = new string[0];
            if (Directory.Exists(path))
            {
                versions = Directory.GetDirectories(path);
            }

            foreach (string versionDirectory in versions)
            {
                var versionPath = System.IO.Path.Combine(versionDirectory, "App.config.json");
                if (!File.Exists(versionPath)) continue;
                var version = JsonConvert.DeserializeObject<AppVersion>(File.ReadAllText(versionPath), new TypeConverter());
                version.Path = System.IO.Path.Combine(versionDirectory, "App.config.json");
                Versions.Add(version);
            }

            // Load general settings file
            var settingsFile = File.ReadAllText(System.IO.Path.Combine(path, "App.info.json"));
            Settings = JsonConvert.DeserializeObject<DMAApplicationVersionInfo>(settingsFile);
        }

        public string Path { get; }

        public string PathSettings { get => System.IO.Path.Combine(Path, "App.info.json"); }

        public List<AppVersion> Versions { get; } = new List<AppVersion>();

        public DMAApplicationVersionInfo Settings { get; }

        public AppVersion LatestVersion
        {
            get => Versions.FirstOrDefault(version => version.Version == Settings.PublicVersion);
        }

        public AppVersion LatestDraftVersion
        {
            get => Versions.FirstOrDefault(version => version.Version == Settings.DraftVersion);
        }
    }
}
