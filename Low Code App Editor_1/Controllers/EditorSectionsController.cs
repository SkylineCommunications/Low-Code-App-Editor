using Low_Code_App_Editor_1.LCA;
using Low_Code_App_Editor_1.UI;
using Newtonsoft.Json.Linq;
using Skyline.DataMiner.Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Low_Code_App_Editor_1.Controllers
{
    public static class EditorSectionsController
    {
        public static void Load(this AppEditorSections editor, App app)
        {
            editor.SelectedApp = app;
            editor.Sections.Clear();
            if(app.Settings.Sections == null)
            {
                app.Settings.Sections = new string[0];
            }

            foreach(var section in app.Settings.Sections)
            {
                var textbox = new RemoveableTextBox(section);
                textbox.RemoveButton.Pressed += (s, ev) =>
                {
                    editor.Sections.Remove(textbox);
                };
                editor.Sections.Add(textbox);
            }
        }

        public static void Save(this AppEditorSections editor)
        {
            var settingsFile = File.ReadAllText(editor.SelectedApp.PathSettings);
            var settings = JObject.Parse(settingsFile);
            var sections = editor.Sections
                .Where(component => component is RemoveableTextBox)
                .Select(box => ((RemoveableTextBox)box).TextBox.Text);
            settings["Sections"] = new JArray(sections);
            File.WriteAllText(editor.SelectedApp.PathSettings, settings.ToString());
        }
    }
}
