// Ignore Spelling: App

namespace Low_Code_App_Editor_1.UI
{
    using Low_Code_App_Editor_1.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorEditors : Dialog<StackPanel>
    {
        public AppEditorEditors(IEngine engine) : base(engine)
        {
            Title = "Edit Editors";

            Panel.Add(Editors);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public AddableStackPanel Editors { get; } = new AddableStackPanel();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
