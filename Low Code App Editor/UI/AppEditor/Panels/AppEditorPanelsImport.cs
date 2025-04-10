// Ignore Spelling: App Apps

namespace Low_Code_App_Editor.UI
{
    using Low_Code_App_Editor.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Web.Common.v1;

    public class AppEditorPanelsImport : Dialog<StackPanel>
    {
        public AppEditorPanelsImport(IEngine engine) : base(engine)
        {
            Title = "Import Panels";

            Panel.Add(Apps);
            Panel.Add(Panels);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public DropDown<App> Apps { get; } = new DropDown<App>();

        public DropDown<DMAApplicationPanel> Panels { get; } = new DropDown<DMAApplicationPanel>();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Import");

        public App SelectedApp { get; set; }
    }
}
