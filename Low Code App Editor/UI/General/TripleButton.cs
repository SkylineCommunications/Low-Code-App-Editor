namespace Low_Code_App_Editor.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class TripleButton : StackPanel
    {
        public TripleButton(string leftText, string middleText, string rightText)
        {
            Direction = Direction.Horizontal;

            LeftButton = new Button(leftText);
            MiddleButton = new Button(middleText);
            RightButton = new Button(rightText);

            Add(LeftButton);
            Add(MiddleButton);
            Add(RightButton);
        }

        public IButton LeftButton { get; }

        public IButton MiddleButton { get; }

        public IButton RightButton { get; }
    }
}
