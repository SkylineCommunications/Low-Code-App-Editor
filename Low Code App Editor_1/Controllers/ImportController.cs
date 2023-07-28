namespace Low_Code_App_Editor_1.Controllers
{
    using System.IO;
    using System.IO.Compression;

    public class ImportController
    {
        public static readonly string ApplicationsDirectory = @"C:\Skyline DataMiner\applications";

        public static bool ImportApp(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                zip.ExtractToDirectory(ApplicationsDirectory);
            }

            return true;
        }
    }
}
