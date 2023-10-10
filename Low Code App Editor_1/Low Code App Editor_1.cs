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
    using System.IO;
    using System.Linq;

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

        private IEngine engine;

        private InteractiveController controller;
        private List<App> apps;

        private AppListOverview overview;
        private ExportDialog export;
        private ImportDialog import;
        private DeleteDialog delete;

        private AppEditor editor;
        private AppEditorSections editorSections;
        private AppEditorEditors editorEditors;
        private AppEditorViewers editorViewers;

        private AppEditorPages editorPages;
        private AppEditorPagesOverview editorPagesOverview;
        private AppEditorPagesImport editorPagesImport;

        private AppEditorPanels editorPanels;
        private AppEditorPanelsOverview editorPanelsOverview;
        private AppEditorPanelsImport editorPanelsImport;

        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            this.engine = engine;
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
            delete = new DeleteDialog(engine);
            editor = new AppEditor(engine);
            editorSections = new AppEditorSections(engine);
            editorEditors = new AppEditorEditors(engine);
            editorViewers = new AppEditorViewers(engine);
            editorPages = new AppEditorPages(engine);
            editorPagesOverview = new AppEditorPagesOverview(engine);
            editorPagesImport = new AppEditorPagesImport(engine);
            editorPanels = new AppEditorPanels(engine);
            editorPanelsOverview = new AppEditorPanelsOverview(engine);
            editorPanelsImport = new AppEditorPanelsImport(engine);
        }

        private void InitUiData(IEngine engine)
        {
            InitOverview(engine);
            InitExport(engine);
            InitImport();
            InitDelete();
            InitEditor();
            InitEditorSections();
            InitEditorEditors();
            InitEditorViewers();
            InitEditorPages();
            InitEditorPagesOverview(engine);
            InitEditorPagesImport(engine);
            InitEditorPanels();
            InitEditorPanelsOverview(engine);
            InitEditorPanelsImport(engine);
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

        private void InitExport(IEngine engine)
        {
            export.BackButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(overview);
            };
            export.ExportButton.Pressed += (sender, e) =>
            {
                var selection = export.Apps.CheckedOptions;
                var file = ExportController.ExportApps(engine, apps.Where(app => app.LatestVersion != null).Where(app => selection.Contains(app.LatestVersion.Name)), ExportOptions.FromDialog(export));
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

            editor.Editors.Pressed += (sender, e) =>
            {
                editorEditors.Load(editor.SelectedApp);
                controller.ShowDialog(editorEditors);
            };

            editor.Viewers.Pressed += (sender, e) =>
            {
                editorViewers.Load(editor.SelectedApp);
                controller.ShowDialog(editorViewers);
            };

            editor.Pages.Pressed += (sender, e) =>
            {
                editorPagesOverview.Load(editorPages, controller, editor.SelectedApp);
                controller.ShowDialog(editorPagesOverview);
            };

            editor.Panels.Pressed += (sender, e) =>
            {
                editorPanelsOverview.Load(editorPanels, controller, editor.SelectedApp);
                controller.ShowDialog(editorPanelsOverview);
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

        private void InitEditorEditors()
        {
            editorEditors.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editor);
            };

            editorEditors.Navigation.RightButton.Pressed += (sender, e) =>
            {
                editorEditors.Save();
                var index = RefreshApp(editorEditors.SelectedApp);
                editor.Load(apps[index]);
                controller.ShowDialog(editor);
            };

            editorEditors.Editors.AddButton.Pressed += (sender, e) =>
            {
                var textbox = new RemoveableTextBox();
                textbox.RemoveButton.Pressed += (s, ev) =>
                {
                    editorEditors.Editors.Remove(textbox);
                };
                editorEditors.Editors.Add(textbox);
            };
        }

        private void InitEditorViewers()
        {
            editorViewers.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editor);
            };

            editorViewers.Navigation.RightButton.Pressed += (sender, e) =>
            {
                editorViewers.Save();
                var index = RefreshApp(editorViewers.SelectedApp);
                editor.Load(apps[index]);
                controller.ShowDialog(editor);
            };

            editorViewers.Viewers.AddButton.Pressed += (sender, e) =>
            {
                var textbox = new RemoveableTextBox();
                textbox.RemoveButton.Pressed += (s, ev) =>
                {
                    editorViewers.Viewers.Remove(textbox);
                };
                editorViewers.Viewers.Add(textbox);
            };
        }

        private void InitEditorPages()
        {
            editorPages.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editorPagesOverview);
            };

            editorPages.Navigation.RightButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editorPagesOverview);
            };
        }

        private void InitEditorPagesOverview(IEngine engine)
        {
            editorPagesOverview.Back.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editor);
            };

            editorPagesOverview.Import.Pressed += (sender, e) =>
            {
                editorPagesImport.Load(editorPagesOverview.SelectedApp, apps, engine);
                controller.ShowDialog(editorPagesImport);
            };
        }

        private void InitEditorPagesImport(IEngine engine)
        {
            editorPagesImport.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editorPagesOverview);
            };

            editorPagesImport.Navigation.RightButton.Pressed += (sender, e) =>
            {
                editorPagesImport.Import(engine);
                editorPagesOverview.Load(editorPages, controller, editorPagesOverview.SelectedApp);
                controller.ShowDialog(editorPagesOverview);
            };

            editorPagesImport.Apps.Changed += (sender, e) =>
            {
                editorPagesImport.LoadPages(e.SelectedOption.Value, engine);
            };
        }

        private void InitEditorPanels()
        {
            editorPanels.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editorPanelsOverview);
            };

            editorPanels.Navigation.RightButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editorPanelsOverview);
            };
        }

        private void InitEditorPanelsOverview(IEngine engine)
        {
            editorPanelsOverview.Back.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editor);
            };

            editorPanelsOverview.Import.Pressed += (sender, e) =>
            {
                editorPanelsImport.Load(editorPanelsOverview.SelectedApp, apps, engine);
                controller.ShowDialog(editorPanelsImport);
            };
        }

        private void InitEditorPanelsImport(IEngine engine)
        {
            editorPanelsImport.Navigation.LeftButton.Pressed += (sender, e) =>
            {
                controller.ShowDialog(editorPanelsOverview);
            };

            editorPanelsImport.Navigation.RightButton.Pressed += (sender, e) =>
            {
                editorPanelsImport.Import(engine);
                editorPanelsOverview.Load(editorPanels, controller, editorPanelsOverview.SelectedApp);
                controller.ShowDialog(editorPanelsOverview);
            };

            editorPanelsImport.Apps.Changed += (sender, e) =>
            {
                editorPanelsImport.LoadPanels(e.SelectedOption.Value, engine);
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
            apps[index] = new App(app.Path, engine);
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
                apps.Add(new App(folder, engine));
            }

            return apps;
        }
    }
}