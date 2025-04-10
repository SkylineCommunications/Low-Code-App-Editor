namespace Low_Code_App_Editor.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Low_Code_App_Editor.LCA;
    using Low_Code_App_Editor.UI;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public static class OverviewController
    {
        public static void Load(this AppListOverview overview, List<App> apps, AppEditor editor, InteractiveController controller)
        {
            overview.Apps.Clear();
            foreach (var app in apps)
            {
                if (app.LatestVersion == null)
                    continue;

                var editButton = new EditButton<App>("Edit...", app);
                editButton.Pressed += (sender, e) =>
                {
                    editor.Load(app);
                    controller.ShowDialog(editor);
                };

                overview.Apps.Add(app.LatestVersion?.Name, editButton);
            }
        }
    }
}
