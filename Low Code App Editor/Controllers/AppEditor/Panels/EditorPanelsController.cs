// Ignore Spelling: app

namespace Low_Code_App_Editor.Controllers
{
    using System;

    using Low_Code_App_Editor.UI;

    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Web.Common.v1;

    public static class EditorPanelsController
    {
        public static void Load(this AppEditorPanels editor, DMAApplicationPanel page)
        {
            editor.PanelFields.Clear();
            editor.PanelFields.Add(nameof(page.ID), new Label(page.ID));
            editor.PanelFields.Add(nameof(page.Name), new Label(page.Name));
            var description = page.Description ?? String.Empty;
            editor.PanelFields.Add(nameof(page.Description), new Label(description));
        }
    }
}
