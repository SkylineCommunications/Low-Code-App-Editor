namespace Low_Code_App_Editor_1.Controllers
{
    using System.IO;
    using System.Linq;

    using Low_Code_App_Editor_1.LCA;
    using Low_Code_App_Editor_1.UI;

    using Newtonsoft.Json.Linq;

    public static class EditorEditorsController
    {
        public static void Load(this AppEditorEditors editor, App app)
        {
            editor.SelectedApp = app;
            editor.Editors.Clear();
            if (app.Settings.Security.AllowEdit == null)
            {
                app.Settings.Security.AllowEdit = new string[0];
            }

            foreach (var person in app.Settings.Security.AllowEdit)
            {
                var textbox = new RemoveableTextBox(person);
                textbox.RemoveButton.Pressed += (s, ev) =>
                {
                    editor.Editors.Remove(textbox);
                };
                editor.Editors.Add(textbox);
            }
        }

        public static void Save(this AppEditorEditors editor)
        {
            var settingsFile = File.ReadAllText(editor.SelectedApp.PathSettings);
            var settings = JObject.Parse(settingsFile);
            var persons = editor.Editors
                .Where(component => component is RemoveableTextBox)
                .Select(box => ((RemoveableTextBox)box).TextBox.Text);
            settings.SelectToken("Security")["AllowEdit"] = new JArray(persons);
            File.WriteAllText(editor.SelectedApp.PathSettings, settings.ToString());
        }
    }
}
