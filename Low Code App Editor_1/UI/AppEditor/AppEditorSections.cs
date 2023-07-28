using Low_Code_App_Editor_1.LCA;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Low_Code_App_Editor_1.UI
{
    public class AppEditorSections : Dialog<StackPanel>
    {
        public AppEditorSections(IEngine engine) : base(engine)
        {
            Title = "Edit Sections";

            Panel.Add(Sections);
            Panel.Add(new WhiteSpace());
            Panel.Add(Navigation);
        }

        public AddableStackPanel Sections { get; } = new AddableStackPanel();

        public DoubleButton Navigation { get; } = new DoubleButton("Back", "Save");

        public App SelectedApp { get; set; }
    }
}
