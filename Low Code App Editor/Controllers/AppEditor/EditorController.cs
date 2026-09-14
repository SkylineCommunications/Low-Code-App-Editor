namespace Low_Code_App_Editor.Controllers
{
	using System.IO;

	using Low_Code_App_Editor.LCA;
	using Low_Code_App_Editor.UI;

	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Automation;

	public static class EditorController
	{
		public static void Load(this AppEditor editor, App app)
		{
			editor.Form.Id.Text = app.LatestVersion.ID;
			editor.Form.Name.Text = app.LatestVersion.Name;
			editor.Form.Description.Text = app.LatestVersion.Description;
			editor.SelectedApp = app;
		}

		public static void Save(this AppEditor editor, IEngine engine)
		{
			var settingsFile = File.ReadAllText(editor.SelectedApp.LatestVersion.Path);
			var settings = JObject.Parse(settingsFile);
			var name = editor.Form.Name.Text;
			var description = editor.Form.Description.Text;
			settings["Name"] = name;
			settings["Description"] = description;
			File.WriteAllText(editor.SelectedApp.LatestVersion.Path, settings.ToString());
			engine.GetUserConnection().SyncFile(editor.SelectedApp.LatestVersion.Path, FileSyncType.Changed);
		}
	}
}
