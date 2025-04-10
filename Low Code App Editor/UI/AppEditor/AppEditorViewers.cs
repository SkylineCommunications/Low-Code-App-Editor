// Ignore Spelling: App

namespace Low_Code_App_Editor.UI
{
    using Low_Code_App_Editor.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorViewers : Dialog<StackPanel>
    {
        public AppEditorViewers(IEngine engine) : base(engine)
        {
            Title = "Edit Viewers";

            Panel.Add(Viewers);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public AddableStackPanel Viewers { get; } = new AddableStackPanel();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
