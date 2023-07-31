namespace Low_Code_App_Editor_1.Package
{
    using Low_Code_App_Editor_1.LCA;
    using System;
    using System.Xml.Serialization;

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
                DisplayName = "Low Code App Install Package",
                Name = "Low Code App Install Package",
                Version = $"1.0.1",
            };
        }
    }
}
