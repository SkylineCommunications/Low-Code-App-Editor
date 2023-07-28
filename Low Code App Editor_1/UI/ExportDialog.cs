namespace Low_Code_App_Editor_1.UI
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class ExportDialog : Dialog<GridPanel>
    {
        public ExportDialog(IEngine engine) : base(engine)
        {
            Title = "Select apps to export";

            ExportPackage.IsChecked = true;

            Panel.Add(Apps, 0, 0);
            Panel.Add(new Label("Include Version History:"), 1, 0);
            Panel.Add(ExportVersions, 1, 1);
            Panel.Add(new Label("Export as Package:"), 2, 0);
            Panel.Add(ExportPackage, 2, 1);
            Panel.Add(new WhiteSpace(), 3, 0);
            Panel.Add(BackButton, 4, 0);
            Panel.Add(ExportButton, 4, 1);
            Panel.Add(Status, 5, 0, 1, 3);
        }

        public ICheckBoxList Apps { get; } = new CheckBoxList();

        public ICheckBox ExportVersions { get; } = new CheckBox();

        public ICheckBox ExportPackage { get; } = new CheckBox();

        public IButton BackButton { get; } = new Button("Back");

        public IButton ExportButton { get; } = new Button("Export");

        public ILabel Status { get; } = new Label(String.Empty);
    }
}
