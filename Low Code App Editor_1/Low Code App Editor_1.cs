/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

26/07/2023	1.0.0.1		AMA, Skyline	Initial version
****************************************************************************
*/

namespace Low_Code_App_Editor_1
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;
    using System.Text;
    using Low_Code_App_Editor_1.Controllers;
    using Low_Code_App_Editor_1.LCA;
    using Low_Code_App_Editor_1.UI;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// engine.ShowUI();
    /// </summary>
    public class Script
    {
        public static readonly string ApplicationsDirectory = @"C:\Skyline DataMiner\applications";

        private InteractiveController controller;
        private List<App> apps;

        private AppListOverview overview;
        private ExportDialog export;
        private ImportDialog import;
        private AppEditor editor;
        private AppEditorSections editorSections;
        private DeleteDialog delete;

        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            controller = new InteractiveController(engine);
            apps = LoadApps();

            InitUiModules(engine);
            InitUiData(engine);

            controller.Run(overview);
        }

        private void InitUiModules(IEngine engine)
        {
            overview = new AppListOverview(engine);
            export = new ExportDialog(engine);
            import = new ImportDialog(engine);
            editor = new AppEditor(engine);
            editorSections = new AppEditorSections(engine);
            delete = new DeleteDialog(engine);
        }

        private void InitUiData(IEngine engine)
        {
            InitOverview(engine);
            InitExport();
            InitImport();
            InitEditor();
            InitEditorSections();
            InitDelete();
        }

        private void InitOverview(IEngine engine)
        {
            overview.Load(apps, editor, controller);
            overview.Refresh.Pressed += (sender, e) =>
            {
                apps = LoadApps();
                overview.Load(apps, editor, controller);
            };
            overview.Export.Pressed += (sender, e) =>
            {
                export.Apps.SetOptions(apps.Where(app => app.LatestVersion != null).Select(app => app.LatestVersion.Name));
                controller.ShowDialog(export);
            };
            overview.Import.Pressed += (sender, e) =>
            {
                controller.ShowDialog(import);
            };
            overview.Delete.Pressed += (sender, e) =>
            {
                delete.Apps.SetOptions(apps.Where(app => app.LatestVersion != null).Select(app => app.LatestVersion.Name));
                controller.ShowDialog(delete);
            };
        }

        private void InitExport()
        {
            export.BackButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(overview);
            };
            export.ExportButton.Pressed += (sender, e) =>
            {
                var selection = export.Apps.CheckedOptions;
                var file = ExportController.ExportApps(apps.Where(app => app.LatestVersion != null).Where(app => selection.Contains(app.LatestVersion.Name)), ExportOptions.FromDialog(export));
                export.Status.Text = $"Exported apps as {file}";
            };
        }

        private void InitImport()
        {
            import.BackButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(overview);
            };

            import.ImportButton.Pressed += (sender, e) =>
            {
                var path = import.FileSelector.UploadedFilePaths.First();
                var result = ImportController.ImportApp(path);
                if (result) import.Status.Text = "Succesfully imported app(s)";
                else import.Status.Text = "An error occured while importing the app(s)";
            };
        }

        private void InitEditor()
        {
            editor.NavigationButtons.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(overview);
            };

            editor.NavigationButtons.RightButton.Pressed += (sender, e) =>
            {
                editor.Save();
                RefreshApp(editor.SelectedApp);
                overview.Load(apps, editor, controller);
                controller.ShowDialog(overview);
            };

            editor.Sections.Pressed += (sender, e) =>
            {
                editorSections.Load(editor.SelectedApp);
                controller.ShowDialog(editorSections);
            };
        }

        private void InitEditorSections()
        {
            editorSections.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editor);
            };

            editorSections.Navigation.RightButton.Pressed += (sender, e) =>
            {
                editorSections.Save();
                var index = RefreshApp(editorSections.SelectedApp);
                editor.Load(apps[index]);
                controller.ShowDialog(editor);
            };

            editorSections.Sections.AddButton.Pressed += (sender, e) =>
            {
                var textbox = new RemoveableTextBox();
                textbox.RemoveButton.Pressed += (s, ev) =>
                {
                    editorSections.Sections.Remove(textbox);
                };
                editorSections.Sections.Add(textbox);
            };
        }

        private void InitDelete()
        {
            delete.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(overview);
            };
            delete.Navigation.RightButton.Pressed += (sender, e) =>
            {
                var selection = delete.Apps.CheckedOptions;
                DeleteController.DeleteApps(apps.Where(app => app.LatestVersion != null).Where(app => selection.Contains(app.LatestVersion.Name)));
                controller.ShowDialog(overview);
            };
        }

        private int RefreshApp(App app)
        {
            var index = apps.IndexOf(app);
            apps[index] = new App(app.Path);
            return index;
        }

        private List<App> LoadApps()
        {
            var apps = new List<App>();
            var folders = new string[0];
            if (Directory.Exists(ApplicationsDirectory))
            {
                folders = Directory.GetDirectories(ApplicationsDirectory);
            }

            foreach (string folder in folders)
            {
                apps.Add(new App(folder));
            }

            return apps;
        }
    }
}