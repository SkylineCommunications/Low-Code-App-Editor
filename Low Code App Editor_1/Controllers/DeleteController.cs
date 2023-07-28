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
                Directory.Delete(app.Path, true);
            }
        }
    }
}
