namespace Low_Code_App_Editor_1.UI
{
    using Low_Code_App_Editor_1.LCA;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditor : Dialog<StackPanel>
    {
        public AppEditor(IEngine engine) : base(engine)
        {
            Title = "Edit App";

            this.Panel.Add(Form);
            this.Panel.Add(Sections);
            this.Panel.Add(new WhiteSpace());
            this.Panel.Add(NavigationButtons);
        }

        public AppEditorFields Form { get; } = new AppEditorFields();

        public IButton Sections { get; } = new Button("Sections...");

        public DoubleButton NavigationButtons { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
