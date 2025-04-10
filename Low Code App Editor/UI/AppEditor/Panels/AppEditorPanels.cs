// Ignore Spelling: App

namespace Low_Code_App_Editor.UI
{
    using Low_Code_App_Editor.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorPanels : Dialog<StackPanel>
    {
        public AppEditorPanels(IEngine engine) : base(engine)
        {
            Title = "Edit Panels";

            Panel.Add(PanelFields);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public IFormPanel PanelFields { get; } = new FormPanel();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
