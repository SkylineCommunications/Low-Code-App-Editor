// Ignore Spelling: App Dms Apps

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
			Panel.Add(new Label("Sync App:"), 3, 0);
			Panel.Add(SyncAppToDms, 3, 1);
			Panel.Add(new Label("Exclude Scripts:"), 4, 0);
			Panel.Add(ExcludeScripts, 4, 1);
			Panel.Add(new Label("Exclude DOM:"), 5, 0);
			Panel.Add(ExcludeDom, 5, 1);
			Panel.Add(new Label("Include DOM Instances:"), 6, 0);
			Panel.Add(ExportDomInstances, 6, 1);
			Panel.Add(new Label("Exclude Images:"), 7, 0);
			Panel.Add(ExcludeImages, 7, 1);
			Panel.Add(new Label("Sync Images:"), 8, 0);
			Panel.Add(SyncImages, 8, 1);
			Panel.Add(new Label("Overwrite Images:"), 9, 0);
			Panel.Add(OverwriteImages, 9, 1);
			Panel.Add(new Label("Exclude Themes:"), 10, 0);
			Panel.Add(ExcludeThemes, 10, 1);
			Panel.Add(new Label("Sync Themes:"), 11, 0);
			Panel.Add(SyncThemes, 11, 1);
			Panel.Add(new Label("Overwrite Themes:"), 12, 0);
			Panel.Add(OverwriteThemes, 12, 1);
			Panel.Add(new WhiteSpace(), 13, 0);
			Panel.Add(BackButton, 14, 0);
			Panel.Add(ExportButton, 14, 1);
			Panel.Add(Status, 15, 0, 1, 3);
		}

		public ICheckBoxList Apps { get; } = new CheckBoxList();

		public ICheckBox ExportVersions { get; } = new CheckBox();

		public ICheckBox OverwritePreviousVersions { get; } = new CheckBox();

		public ICheckBox SyncAppToDms { get; } = new CheckBox { IsChecked = true };

		public ICheckBox ExcludeScripts { get; } = new CheckBox();

		public ICheckBox ExcludeDom { get; } = new CheckBox();

		public ICheckBox ExportDomInstances { get; } = new CheckBox();

		public ICheckBox ExcludeImages { get; } = new CheckBox();

		public ICheckBox SyncImages { get; } = new CheckBox { IsChecked = true };

		public ICheckBox OverwriteImages { get; } = new CheckBox { IsChecked = true };

		public ICheckBox ExcludeThemes { get; } = new CheckBox();

		public ICheckBox SyncThemes { get; } = new CheckBox { IsChecked = true };

		public ICheckBox OverwriteThemes { get; } = new CheckBox { IsChecked = true };

		public IButton BackButton { get; } = new Button("Back");

		public IButton ExportButton { get; } = new Button("Export");

		public ILabel Status { get; } = new Label(String.Empty);
	}
}
