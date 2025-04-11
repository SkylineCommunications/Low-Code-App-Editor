﻿namespace Low_Code_App_Editor.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class EditButton<T> : Button
    {
        public EditButton(T info) : base("Edit...")
        {
            Info = info;
        }

        public EditButton(string text, T info) : base(text)
        {
            Info = info;
        }

        public T Info { get; }
    }
}
