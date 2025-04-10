namespace Low_Code_App_Editor.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class AddableStackPanel : StackPanel
    {
        public AddableStackPanel()
        {
            base.Add(AddButton);
        }

        public IButton AddButton { get; } = new Button("+");

        public new void Add(IComponent component)
        {
            Insert(Count - 1, component);
        }

        public new void Clear()
        {
            base.Clear();
            base.Add(AddButton);
        }
    }
}
