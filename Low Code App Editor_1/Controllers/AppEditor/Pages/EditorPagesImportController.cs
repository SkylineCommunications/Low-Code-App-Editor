// Ignore Spelling: app

namespace Low_Code_App_Editor_1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Low_Code_App_Editor_1.Json;
    using Low_Code_App_Editor_1.LCA;
    using Low_Code_App_Editor_1.UI;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Concatenation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Web.Common.v1;

    public static class EditorPagesImportController
    {
        public static void Load(this AppEditorPagesImport editor, App selectedApp, IEnumerable<App> apps, IEngine engine)
        {
            editor.SelectedApp = selectedApp;
            editor.Apps.Options.Clear();
            editor.Apps.Options.AddRange(apps.Select(app => Option.Create(app.Name, app)));

            var defaultSelected = editor.Apps.Options.FirstOrDefault();
            if(defaultSelected == null)
            {
                return;
            }

            editor.Apps.SelectedValue = defaultSelected.Value;

            editor.LoadPages(defaultSelected.Value, engine);
        }

        public static void LoadPages(this AppEditorPagesImport editor, App app, IEngine engine)
        {
            editor.Pages.Options.Clear();
            editor.Pages.Options.AddRange(app.LatestVersion.Pages.Select(page => Option.Create(page.Name, page)));
        }

        public static void Import(this AppEditorPagesImport editor, IEngine engine)
        {
            var selectedApp = editor.Apps.SelectedValue;
            var selectedPage = editor.Pages.SelectedValue;

            // Check if draft version needs to be updated too
            if(editor.SelectedApp.LatestDraftVersion != null)
            {
                ImportIntoVersion(selectedApp.LatestVersion, selectedPage, editor.SelectedApp.LatestDraftVersion);
            }

            // Import page into latest version
            ImportIntoVersion(selectedApp.LatestVersion, selectedPage, editor.SelectedApp.LatestVersion);
        }

        private static void ImportIntoVersion(AppVersion fromAppVersion, DMAApplicationPage fromPage, AppVersion toAppVersion)
        {
            // Grab json configurations
            var selectedAppJson = JObject.Parse(File.ReadAllText(fromAppVersion.Path));
            var selectedPageJson = ((JArray)selectedAppJson.SelectToken("Pages")).FirstOrDefault(token => token["Name"].Value<string>() == fromPage.Name);
            if (selectedPageJson == null)
            {
                throw new FileNotFoundException($"The page from app '{fromAppVersion.Name}' called '{fromPage.Name}', was not found.");
            }

            // Copy over page
            File.Copy(
                Path.Combine(fromAppVersion.FolderPath, "pages", $"{fromPage.ID}.dmadb.json"),
                Path.Combine(toAppVersion.FolderPath, "pages", $"{fromPage.ID}.dmadb.json"));

            // Edit the latest config to include the newly added page
            var config = JObject.Parse(File.ReadAllText(toAppVersion.Path));
            var pagesToken = config.SelectToken("Pages");
            if (pagesToken.Type == JTokenType.Null)
            {
                pagesToken = new JArray();
            }

            var pages = (JArray)pagesToken;
            pages.Add(selectedPageJson);
            config["Pages"] = pages;

            // Update our local copy of the app
            var localPages = toAppVersion.Pages.ToList();
            localPages.Add(fromPage);
            toAppVersion.Pages = localPages.ToArray();

            // Save the config
            File.WriteAllText(
                Path.Combine(toAppVersion.Path),
                config.ToString(Formatting.None));
        }
    }
}
