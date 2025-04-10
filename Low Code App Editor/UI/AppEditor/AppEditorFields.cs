namespace Low_Code_App_Editor.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AppEditorFields : FormPanel
    {
        public AppEditorFields()
        {
            this.Add("ID", Id);
            this.Add("Name", Name);
            this.Add("Description", Description);
        }

        public ILabel Id { get; } = new Label();

        public ITextBox Name { get; } = new TextBox();

        public ITextBox Description { get; } = new TextBox();
    }
}
