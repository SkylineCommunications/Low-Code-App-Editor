namespace Low_Code_App_Editor.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class RemoveableTextBox : StackPanel
    {
        public RemoveableTextBox()
        {
            Direction = Direction.Horizontal;
            Add(TextBox);
            Add(RemoveButton);
        }

        public RemoveableTextBox(string value)
        {
            Direction = Direction.Horizontal;
            TextBox.Text = value;

            Add(TextBox);
            Add(RemoveButton);
        }

        public ITextBox TextBox { get; } = new TextBox();

        public IButton RemoveButton { get; } = new Button("x");
    }
}
