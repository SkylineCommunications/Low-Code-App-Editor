namespace Low_Code_App_Editor_1.UI
{
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppListOverview : Dialog<StackPanel>
    {
        public AppListOverview(IEngine engine) : base(engine)
        {
            Title = "Apps";
            Panel.Add(Apps);
            // Panel.Add(AddButton);
            Panel.Add(new WhiteSpace());
            Panel.Add(new StackPanel(Direction.Horizontal) { Refresh, Import, Export, Delete, });
        }

        public FormPanel Apps { get; } = new FormPanel();

        public IButton AddButton { get; } = new Button("New...");

        public IButton Refresh { get; } = new Button("Refresh");

        public IButton Import { get; } = new Button("Import...");

        public IButton Export { get; } = new Button("Export...");

        public IButton Delete { get; } = new Button("Delete...");
    }
}