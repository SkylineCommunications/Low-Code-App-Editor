namespace Low_Code_App_Editor_1.Package
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Low_Code_App_Editor_1.LCA;

    [XmlRoot(ElementName = "AppInfo")]
    public class PackageInfo
    {
        [XmlElement(ElementName = "Build")]
        public int Build { get; set; }

        [XmlElement(ElementName = "DisplayName")]
        public string DisplayName { get; set; }

        [XmlElement(ElementName = "LastModifiedAt")]
        public DateTime LastModifiedAt { get; set; }

        [XmlElement(ElementName = "MinDmaVersion")]
        public string MinDmaVersion { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "AllowMultipleInstalledVersions")]
        public bool AllowMultipleInstalledVersions { get; set; }

        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }

        public static PackageInfo FromApp()
        {
            return new PackageInfo
            {
                Build = 1,
                AllowMultipleInstalledVersions = false,
                LastModifiedAt = DateTime.Now,
                MinDmaVersion = "10.0.10.0-9414",
                DisplayName = $"Low Code App Install Package",
                Name = $"Low Code App Install Package",
                Version = $"1.0.1",
            };
        }

        public static PackageInfo FromApp(App app)
        {
            var info = PackageInfo.FromApp();
            info.DisplayName = $"{app.Name} App Install Pacakge";
            info.Name = $"{app.Name} App Install Pacakge";
            return info;
        }
    }
}
