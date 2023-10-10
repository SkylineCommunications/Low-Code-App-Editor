// Ignore Spelling: app

namespace Low_Code_App_Editor_1.Controllers
{
    using System;

    using Low_Code_App_Editor_1.UI;

    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Web.Common.v1;

    public static class EditorPagesController
    {
        public static void Load(this AppEditorPages editor, DMAApplicationPage page)
        {
            editor.PageFields.Clear();
            editor.PageFields.Add(nameof(page.ID), new Label(page.ID));
            editor.PageFields.Add(nameof(page.Name), new Label(page.Name));
            var description = page.Description ?? String.Empty;
            editor.PageFields.Add(nameof(page.Description), new Label(description));
            editor.PageFields.Add(nameof(page.Hidden), new Label(page.Hidden.ToString()));
        }
    }
}
