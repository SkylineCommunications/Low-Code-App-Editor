namespace Low_Code_App_Editor.UI.General
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class ConfirmationDialog : Dialog<StackPanel>
    {
        private readonly ILabel message = new Label();

        public ConfirmationDialog(IEngine engine, string text) : base(engine)
        {
            message.Text = text;

            Panel.Add(message);
            Panel.Add(new WhiteSpace());
            Panel.Add(new StackPanel(Direction.Horizontal) { Ok, Cancel });
        }

        public IButton Ok { get; } = new Button("Ok");

        public IButton Cancel { get; } = new Button("Cancel");
    }
}
