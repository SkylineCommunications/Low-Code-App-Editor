namespace Low_Code_App_Editor_1.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorFields : FormPanel
    {
        public AppEditorFields()
        {
            this.Add("Name", Name);
            this.Add("Description", Description);
        }

        public ITextBox Name { get; } = new TextBox();

        public ITextBox Description { get; } = new TextBox();
    }
}
