namespace Low_Code_App_Editor_1.UI
{
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class EditButton<T> : Button
    {
        public EditButton(string text, T info) : base(text)
        {
            Info = info;
        }

        public T Info { get; }
    }
}
