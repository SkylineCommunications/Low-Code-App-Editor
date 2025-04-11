// Ignore Spelling: App

namespace Low_Code_App_Editor.UI
{
    using Low_Code_App_Editor.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorPages : Dialog<StackPanel>
    {
        public AppEditorPages(IEngine engine) : base(engine)
        {
            Title = "Edit Pages";

            Panel.Add(PageFields);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public IFormPanel PageFields { get; } = new FormPanel();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
