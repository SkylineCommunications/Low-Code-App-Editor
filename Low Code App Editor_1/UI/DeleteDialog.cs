namespace Low_Code_App_Editor_1.UI
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class DeleteDialog : Dialog<StackPanel>
    {
        public DeleteDialog(IEngine engine) : base(engine)
        {
            Title = "Select apps to delete";
            Panel.Add(Apps);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
            Panel.Add(Status);
        }

        public ICheckBoxList Apps { get; } = new CheckBoxList();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Delete");

        public ILabel Status { get; } = new Label(String.Empty);
    }
}
