// Ignore Spelling: App

namespace Low_Code_App_Editor_1.UI
{
    using Low_Code_App_Editor_1.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorPanelsOverview : Dialog<StackPanel>
    {
        public AppEditorPanelsOverview(IEngine engine) : base(engine)
        {
            Title = "Edit Pages";

            Panel.Add(Panels);
            Panel.Add(Import);
            Panel.Add(new WhiteSpace());
            Panel.Add(Back);
        }

        public IFormPanel Panels { get; } = new FormPanel();

        public IButton Import { get; } = new Button("Import...");

        public IButton Back { get; } = new Button("Back");

        public App SelectedApp { get; set; }
    }
}
