namespace Low_Code_App_Editor_1.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using Low_Code_App_Editor_1.LCA;

    public class DeleteController
    {
        public static void DeleteApps(IEnumerable<App> apps)
        {
            foreach(var app in apps)
            {
                DeleteDirectory(app.Path);
            }
        }

        private static void DeleteDirectory(string directory)
        {
            var files = Directory.GetFiles(directory);
            var dirs = Directory.GetDirectories(directory);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(directory);
        }
    }
}
