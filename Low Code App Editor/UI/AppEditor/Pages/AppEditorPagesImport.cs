// Ignore Spelling: App Apps

namespace Low_Code_App_Editor.UI
{
    using Low_Code_App_Editor.LCA;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using Skyline.DataMiner.Web.Common.v1;

    public class AppEditorPagesImport : Dialog<StackPanel>
    {
        public AppEditorPagesImport(IEngine engine) : base(engine)
        {
            Title = "Import Pages";

            Panel.Add(Apps);
            Panel.Add(Pages);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public DropDown<App> Apps { get; } = new DropDown<App>();

        public DropDown<DMAApplicationPage> Pages { get; } = new DropDown<DMAApplicationPage>();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Import");

        public App SelectedApp { get; set; }
    }
}
