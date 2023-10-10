namespace Low_Code_App_Editor_1.Controllers
{
    using System.IO;
    using System.Linq;

    using Low_Code_App_Editor_1.LCA;
    using Low_Code_App_Editor_1.UI;

    using Newtonsoft.Json.Linq;

    public static class EditorViewersController
    {
        public static void Load(this AppEditorViewers editor, App app)
        {
            editor.SelectedApp = app;
            editor.Viewers.Clear();
            if (app.Settings.Security.AllowView == null)
            {
                app.Settings.Security.AllowView = new string[0];
            }

            foreach (var person in app.Settings.Security.AllowView)
            {
                var textbox = new RemoveableTextBox(person);
                textbox.RemoveButton.Pressed += (s, ev) =>
                {
                    editor.Viewers.Remove(textbox);
                };
                editor.Viewers.Add(textbox);
            }
        }

        public static void Save(this AppEditorViewers editor)
        {
            var settingsFile = File.ReadAllText(editor.SelectedApp.PathSettings);
            var settings = JObject.Parse(settingsFile);
            var persons = editor.Viewers
                .Where(component => component is RemoveableTextBox)
                .Select(box => ((RemoveableTextBox)box).TextBox.Text);
            settings.SelectToken("Security")["AllowView"] = new JArray(persons);
            File.WriteAllText(editor.SelectedApp.PathSettings, settings.ToString());
        }
    }
}
