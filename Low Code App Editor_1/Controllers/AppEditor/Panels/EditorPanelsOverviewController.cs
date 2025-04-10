// Ignore Spelling: app

namespace Low_Code_App_Editor_1.Controllers
{
	using System;
	using System.IO;
	using System.Linq;

	using Low_Code_App_Editor_1.LCA;
	using Low_Code_App_Editor_1.UI;
	using Low_Code_App_Editor_1.UI.General;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Web.Common.v1;

	public static class EditorPanelsOverviewController
	{
		public static void Load(this AppEditorPanelsOverview editor, AppEditorPanels panelEditor, InteractiveController controller, App app)
		{
			editor.SelectedApp = app;
			editor.Panels.Clear();
			if (app.LatestVersion.Panels == null)
			{
				app.LatestVersion.Panels = new Skyline.DataMiner.Web.Common.v1.DMAApplicationPanel[0];
			}

			foreach (var panel in app.LatestVersion.Panels)
			{
				var editButton = new TripleButton("Edit...", "Duplicate...", "Delete...");
				editButton.LeftButton.Pressed += (sender, e) =>
				{
					panelEditor.Load(panel);
					controller.ShowDialog(panelEditor);
				};
				editButton.MiddleButton.Pressed += (sender, e) =>
				{
					var confirmation = new ConfirmationDialog(controller.Engine, $"Are you sure you want to duplicate panel '{panel.Name}'?");
					confirmation.Ok.Pressed += (s, ev) =>
					{
						Duplicate(app, panel);
						editor.Load(panelEditor, controller, app);
						controller.ShowDialog(editor);
					};
					confirmation.Cancel.Pressed += (s, ev) =>
					{
						controller.ShowDialog(editor);
					};
					controller.ShowDialog(confirmation);
				};
				editButton.RightButton.Pressed += (sender, e) =>
				{
					var confirmation = new ConfirmationDialog(controller.Engine, $"Are you sure you want to delete panel '{panel.Name}' from the app '{app.Name}'?");
					confirmation.Ok.Pressed += (s, ev) =>
					{
						Delete(app, panel);
						editor.Load(panelEditor, controller, app);
						controller.ShowDialog(editor);
					};
					confirmation.Cancel.Pressed += (s, ev) =>
					{
						controller.ShowDialog(editor);
					};
					controller.ShowDialog(confirmation);
				};

				editor.Panels.Add(panel.Name, editButton);
			}
		}

		public static void Duplicate(App app, DMAApplicationPanel panel)
		{
			var newGuid = Convert.ToString(Guid.NewGuid());

			// Create the paths
			var originalPath = Path.Combine(app.LatestVersion.FolderPath, "pages", $"{panel.ID}.dmadb.json");
			var duplicatePath = Path.Combine(app.LatestVersion.FolderPath, "pages", $"{newGuid}.dmadb.json");

			// Parse the config
			var config = JObject.Parse(File.ReadAllText(app.LatestVersion.Path));
			var panelsToken = config.SelectToken("Panels");
			var panels = (JArray)panelsToken;

			var selectedPanel = panels.FirstOrDefault(p => p["ID"].Value<string>() == panel.ID);
			if (selectedPanel is null)
			{
				throw new LowCodeAppEditorException("Could not find the selected panel in the config of the app.");
			}

			if (!File.Exists(originalPath))
			{
				throw new LowCodeAppEditorException("Could not find the selected panel in the pages folder of the app.");
			}

			// Read out the page and to the edits for the new id
			var content = File.ReadAllText(originalPath)
				.Replace(panel.ID, newGuid);

			// Create the duplicate panel file
			File.WriteAllText(duplicatePath, content);

			// Create the duplicate config enytry
			var newPage = selectedPanel.DeepClone();
			newPage["ID"] = newGuid;
			newPage["Name"] = $"{newPage["Name"].Value<string>()} - Copy";

			var rawPage = newPage.ToString(Formatting.None)
				.Replace(panel.ID, newGuid);
			newPage = JToken.Parse(rawPage);

			panels.Add(newPage);
			config["Panels"] = panels;

			// Save the config
			File.WriteAllText(
				Path.Combine(app.LatestVersion.Path),
				config.ToString(Formatting.None));

			// Update our local copy of the app
			var latestApp = new App(app.Path);
			app.LatestVersion.Panels = latestApp.LatestVersion.Panels;
		}

		public static void Delete(App app, DMAApplicationPanel panel)
		{
			// Delete from latest version
			DeleteFromVersion(app.LatestVersion, panel);

			// Delete from draft version
			if (app.LatestDraftVersion != null)
			{
				DeleteFromVersion(app.LatestDraftVersion, panel);
			}
		}

		private static void DeleteFromVersion(AppVersion version, DMAApplicationPanel panel)
		{
			// Remove the panels file
			File.Delete(Path.Combine(version.FolderPath, "pages", $"{panel.ID}.dmadb.json"));

			// Remove reference from config
			var config = JObject.Parse(File.ReadAllText(version.Path));
			var panels = (JArray)config.SelectToken("Panels");
			var selectedPanelIndex = panels.FirstOrDefault(token => token["ID"].Value<string>() == panel.ID);
			panels.Remove(selectedPanelIndex);
			config["Panels"] = panels;
			File.WriteAllText(version.Path, config.ToString(Formatting.None));

			// Remove from local copy
			var localPanel = version.Panels.ToList();
			localPanel.Remove(version.Panels.FirstOrDefault(pnl => pnl.ID == panel.ID));
			version.Panels = localPanel.ToArray();
		}
	}
}
