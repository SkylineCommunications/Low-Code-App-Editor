// Ignore Spelling: app

namespace Low_Code_App_Editor_1.Controllers
{
    using System.IO;
    using System.Linq;

    using Low_Code_App_Editor_1.LCA;
    using Low_Code_App_Editor_1.UI;
    using Low_Code_App_Editor_1.UI.General;

    using Newtonsoft.Json.Linq;

    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Web.Common.v1;

    public static class EditorPagesOverviewController
    {
        public static void Load(this AppEditorPagesOverview editor, AppEditorPages pageEditor, InteractiveController controller, App app)
        {
            editor.SelectedApp = app;
            editor.Pages.Clear();
            if(app.LatestVersion.Pages == null)
            {
                app.LatestVersion.Pages = new Skyline.DataMiner.Web.Common.v1.DMAApplicationPage[0];
            }

            foreach(var page in app.LatestVersion.Pages)
            {
                var editButton = new DoubleButton("Edit...", "Delete");
                editButton.LeftButton.Pressed += (sender, e) =>
                {
                    pageEditor.Load(page);
                    controller.ShowDialog(pageEditor);
                };
                editButton.RightButton.Pressed += (sender, e) =>
                {
                    var confirmation = new ConfirmationDialog(controller.Engine, $"Are you sure you want to delete page '{page.Name}' from the app '{app.Name}'?");
                    confirmation.Ok.Pressed += (s, ev) =>
                    {
                        Delete(app, page);
                        editor.Load(pageEditor, controller, app);
                        controller.ShowDialog(editor);
                    };
                    confirmation.Cancel.Pressed += (s, ev) =>
                    {
                        controller.ShowDialog(editor);
                    };
                    controller.ShowDialog(confirmation);
                };

                editor.Pages.Add(page.Name, editButton);
            }
        }

        public static void Delete(App app, DMAApplicationPage page)
        {
            // Delete from latest version
            DeleteFromVersion(app.LatestVersion, page);

            // Delete from draft version
            if(app.LatestDraftVersion != null)
            {
                DeleteFromVersion(app.LatestDraftVersion, page);
            }
        }

        private static void DeleteFromVersion(AppVersion version, DMAApplicationPage page)
        {
            // Remove the pages file
            File.Delete(Path.Combine(version.FolderPath, "pages", $"{page.ID}.dmadb.json"));

            // Remove reference from config
            var config = JObject.Parse(File.ReadAllText(version.Path));
            var pages = (JArray)config.SelectToken("Pages");
            var selectedPageIndex = pages.FirstOrDefault(token => token["ID"].Value<string>() == page.ID);
            pages.Remove(selectedPageIndex);
            config["Pages"] = pages;
            File.WriteAllText(version.Path, config.ToString(Newtonsoft.Json.Formatting.None));

            // Remove from local copy
            var localPages = version.Pages.ToList();
            localPages.Remove(version.Pages.FirstOrDefault(p => p.ID == page.ID));
            version.Pages = localPages.ToArray();
        }
    }
}
