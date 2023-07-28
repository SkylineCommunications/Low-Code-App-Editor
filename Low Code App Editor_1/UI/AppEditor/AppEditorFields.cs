using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Low_Code_App_Editor_1.UI
{
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
