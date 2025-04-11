namespace Low_Code_App_Editor.UI
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class ImportDialog : Dialog<GridPanel>
    {
        public ImportDialog(IEngine engine) : base(engine)
        {
            Title = "Select a file to import";
            Panel.Add(FileSelector, 0, 0);
            Panel.Add(new WhiteSpace(), 1, 0);
            Panel.Add(BackButton, 2, 0);
            Panel.Add(ImportButton, 2, 1);
            Panel.Add(Status, 3, 0, 1, 3);
        }

        public IFileSelector FileSelector { get; } = new FileSelector();

        public IButton ImportButton { get; } = new Button("Import");

        public IButton BackButton { get; } = new Button("Back");

        public ILabel Status { get; } = new Label(String.Empty);
    }
}
