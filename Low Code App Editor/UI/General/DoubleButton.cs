namespace Low_Code_App_Editor.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class DoubleButton : StackPanel
    {
        public DoubleButton(string leftText, string rightText)
        {
            Direction = Direction.Horizontal;

            LeftButton = new Button(leftText);
            RightButton = new Button(rightText);

            Add(LeftButton);
            Add(RightButton);
        }

        public IButton LeftButton { get; }

        public IButton RightButton { get; }
    }
}
