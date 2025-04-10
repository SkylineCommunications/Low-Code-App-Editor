// Ignore Spelling: App

namespace Low_Code_App_Editor.UI
{
    using Low_Code_App_Editor.LCA;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditor : Dialog<StackPanel>
    {
        public AppEditor(IEngine engine) : base(engine)
        {
            Title = "Edit App";

            this.Panel.Add(Form);
            this.Panel.Add(Sections);
            this.Panel.Add(Editors);
            this.Panel.Add(Viewers);
            this.Panel.Add(Pages);
            this.Panel.Add(Panels);
            this.Panel.Add(new WhiteSpace());
            this.Panel.Add(NavigationButtons);
        }

        public AppEditorFields Form { get; } = new AppEditorFields();

        public IButton Sections { get; } = new Button("Sections...");

        public IButton Editors { get; } = new Button("Editors...");

        public IButton Viewers { get; } = new Button("Viewers...");

        public IButton Pages { get; } = new Button("Pages...");

        public IButton Panels { get; } = new Button("Panels...");

        public DoubleButton NavigationButtons { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
