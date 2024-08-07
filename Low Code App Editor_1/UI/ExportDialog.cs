namespace Low_Code_App_Editor_1.UI
{
	using System;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class ExportDialog : Dialog<GridPanel>
	{
		public ExportDialog(IEngine engine) : base(engine)
		{
			Title = "Select apps to export";

			Panel.Add(Apps, 0, 0);
			Panel.Add(new Label("Include Version History:"), 1, 0);
			Panel.Add(ExportVersions, 1, 1);
			Panel.Add(new Label("Overwrite Previous Versions:"), 2, 0);
			Panel.Add(OverwritePreviousVersions, 2, 1);
			Panel.Add(new Label("Exclude Scripts:"), 3, 0);
			Panel.Add(ExcludeScripts, 3, 1);
			Panel.Add(new Label("Exclude DOM:"), 4, 0);
			Panel.Add(ExcludeDom, 4, 1);
			Panel.Add(new Label("Export DOM Instances:"), 5, 0);
			Panel.Add(ExportDomInstances, 5, 1);
			Panel.Add(new Label("Exclude Images:"), 6, 0);
			Panel.Add(ExcludeImages, 6, 1);
			Panel.Add(new Label("Exclude Themes:"), 7, 0);
			Panel.Add(ExcludeThemes, 7, 1);
			Panel.Add(new WhiteSpace(), 8, 0);
			Panel.Add(BackButton, 9, 0);
			Panel.Add(ExportButton, 10, 1);
			Panel.Add(Status, 11, 0, 1, 3);
		}

		public ICheckBoxList Apps { get; } = new CheckBoxList();

		public ICheckBox ExportVersions { get; } = new CheckBox();

		public ICheckBox OverwritePreviousVersions { get; } = new CheckBox();

		public ICheckBox ExcludeScripts { get; } = new CheckBox();

		public ICheckBox ExcludeDom { get; } = new CheckBox();

		public ICheckBox ExportDomInstances { get; } = new CheckBox();

		public ICheckBox ExcludeImages { get; } = new CheckBox();

		public ICheckBox ExcludeThemes { get; } = new CheckBox();

		public IButton BackButton { get; } = new Button("Back");

		public IButton ExportButton { get; } = new Button("Export");

		public ILabel Status { get; } = new Label(String.Empty);
	}
}
