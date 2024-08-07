// Ignore Spelling: app

namespace Low_Code_App_Editor_1.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Low_Code_App_Editor_1.LCA;
	using Low_Code_App_Editor_1.UI;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Concatenation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Web.Common.v1;

	public static class EditorPanelsImportController
	{
		public static void Load(this AppEditorPanelsImport editor, App selectedApp, IEnumerable<App> apps, IEngine engine)
		{
			editor.SelectedApp = selectedApp;
			editor.Apps.Options.Clear();
			editor.Apps.Options.AddRange(apps.Select(app => Option.Create($"[{app.LatestVersion.ID}] - {app.Name}", app)));

			var defaultSelected = editor.Apps.Options.FirstOrDefault();
			if (defaultSelected == null)
			{
				return;
			}

			editor.Apps.SelectedValue = defaultSelected.Value;

			editor.LoadPanels(defaultSelected.Value, engine);
		}

		public static void LoadPanels(this AppEditorPanelsImport editor, App app, IEngine engine)
		{
			editor.Panels.Options.Clear();
			editor.Panels.Options.AddRange(app.LatestVersion.Panels.Select(panel => Option.Create(panel.Name, panel)));
		}

		public static void Import(this AppEditorPanelsImport editor, IEngine engine)
		{
			var selectedApp = editor.Apps.SelectedValue;
			var selectedPanel = editor.Panels.SelectedValue;

			// Check if draft page needs to be updated too
			if (editor.SelectedApp.LatestDraftVersion != null)
			{
				ImportIntoVersion(selectedApp.LatestVersion, selectedPanel, editor.SelectedApp.LatestDraftVersion);
			}

			// Import panel into latest version
			ImportIntoVersion(selectedApp.LatestVersion, selectedPanel, editor.SelectedApp.LatestVersion);
		}

		private static void ImportIntoVersion(AppVersion fromAppVersion, DMAApplicationPanel fromPanel, AppVersion toAppVersion)
		{
			// Grab json configurations
			var selectedAppJson = JObject.Parse(File.ReadAllText(fromAppVersion.Path));
			var selectedPanelJson = ((JArray)selectedAppJson.SelectToken("Panels")).FirstOrDefault(token => token["Name"].Value<string>() == fromPanel.Name);
			if (selectedPanelJson == null)
			{
				throw new FileNotFoundException($"The panel from app '{fromAppVersion.Name}' called '{fromPanel.Name}', was not found.");
			}

			// Copy over page
			File.Copy(
				Path.Combine(fromAppVersion.FolderPath, "pages", $"{fromPanel.ID}.dmadb.json"),
				Path.Combine(toAppVersion.FolderPath, "pages", $"{fromPanel.ID}.dmadb.json"));

			// Edit the latest config to include the newly added page
			var config = JObject.Parse(File.ReadAllText(toAppVersion.Path));
			var panelsToken = config.SelectToken("Panels");
			if (panelsToken.Type == JTokenType.Null)
			{
				panelsToken = new JArray();
			}

			var panels = (JArray)panelsToken;
			panels.Add(selectedPanelJson);
			config["Panels"] = panels;

			// Update our local copy of the app
			var localPanels = toAppVersion.Panels.ToList();
			localPanels.Add(fromPanel);
			toAppVersion.Panels = localPanels.ToArray();

			// Save the config
			File.WriteAllText(
				Path.Combine(toAppVersion.Path),
				config.ToString(Formatting.None));
		}
	}
}