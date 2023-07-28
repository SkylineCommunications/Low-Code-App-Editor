namespace Low_Code_App_Editor_1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Newtonsoft.Json.Linq;

    public static class Extensions
    {
        /// <summary>
        /// Creates a new entry in the specified <see cref="ZipArchive"/> from the contents of the provided directory.
        /// </summary>
        /// <param name="archive">The target ZipArchive to which the entry will be added.</param>
        /// <param name="directoryPath">The path of the source directory whose contents will be included in the ZipArchive entry.</param>
        /// <param name="entryPath">The optional path of the new entry to be created within the ZipArchive. If not specified, the entry will be created with the same name as the source directory.</param>
        /// <param name="recursiveCreate">Determines whether to include subdirectories and their contents recursively in the ZipArchive entry. Default value is true.</param>
        /// <remarks>
        /// This method creates a new entry within the specified ZipArchive and includes all files and subdirectories within the source directory.
        /// If <paramref name="entryPath"/> is null or empty, the entry will be created with the same name as the source directory.
        /// If <paramref name="recursiveCreate"/> is set to true, all subdirectories and their contents will be included recursively in the ZipArchive entry.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="archive"/> or <paramref name="directoryPath"/> is null or empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the <paramref name="directoryPath"/> does not exist.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs while reading the files or writing to the ZipArchive.</exception>
        public static void CreateEntryFromDirectory(this ZipArchive archive, string directoryPath, string entryPath = null, bool recursiveCreate = true)
        {
            if (entryPath == null)
            {
                entryPath = Path.GetFileName(directoryPath);
            }

            DirectoryInfo d = new DirectoryInfo(directoryPath);
            FileInfo[] files = d.GetFiles("*");
            foreach (FileInfo file in files)
            {
                archive.CreateEntryFromFile(file.FullName, entryPath + "/" + file.Name);
            }

            if (!recursiveCreate)
                return;

            DirectoryInfo[] folders = d.GetDirectories("*");
            foreach (DirectoryInfo folder in folders)
            {
                archive.CreateEntryFromDirectory(folder.FullName, $"{entryPath}/{folder.Name}", true);
            }
        }

        public static void CreateEntryFromText(this ZipArchive archive, string entryPath, string text)
        {
            // Create a new entry (file) in the zip archive
            ZipArchiveEntry entry = archive.CreateEntry(entryPath);

            // Write the string content to the entry (file) in the zip archive
            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                writer.Write(text);
            }
        }

        public static IEnumerable<ZipArchiveEntry> GetEntries(this ZipArchive archive, string entryPath = "")
        {
            return archive.Entries.Where(entry => entry.FullName.StartsWith(entryPath)).ToList();
        }

        public static List<JToken> FindPropertiesWithName(this JToken token, string propertyName)
        {
            var scriptProperties = new List<JToken>();
            if (token is JProperty property)
            {
                if (property.Name == propertyName)
                {
                    // Add the property and its parent to the list
                    scriptProperties.Add(property.Value);
                }

                // Continue recursion for the property's value
                scriptProperties.AddRange(property.Value.FindPropertiesWithName(propertyName));
            }
            else if (token is JObject obj)
            {
                // Continue recursion for all the object's properties
                foreach (JProperty childProperty in obj.Properties())
                {
                    scriptProperties.AddRange(childProperty.FindPropertiesWithName(propertyName));
                }
            }
            else if (token is JArray array)
            {
                // Continue recursion for all the array items
                foreach (JToken item in array)
                {
                    scriptProperties.AddRange(item.FindPropertiesWithName(propertyName));
                }
            }

            return scriptProperties;
        }
    }

    public static class XmlConvert
    {
        public static string SerializeObject<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = new XmlTextWriter(sww) { Formatting = System.Xml.Formatting.Indented })
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    serializer.Serialize(writer, obj, namespaces);
                    return sww.ToString();
                }
            }
        }
    }
}
