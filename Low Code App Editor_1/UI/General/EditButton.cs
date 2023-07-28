using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Low_Code_App_Editor_1.UI
{
    public class EditButton<T> : Button
    {
        public EditButton(string text, T info) : base(text)
        {
            Info = info;
        }

        public T Info { get; }
    }
}
