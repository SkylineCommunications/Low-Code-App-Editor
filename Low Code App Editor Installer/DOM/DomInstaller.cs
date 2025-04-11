namespace Install_1.DOM
{
    using System;
    using System.IO;
    using Install_1;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.AppPackages;

    public class DomInstaller
	{
		private const string DOM_FOLDERPATH = @"CompanionFiles\DOM\";

		private readonly AppInstallContext context;
		private readonly Action<string> logMethod;

		private readonly DomImporter domImporter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DomInstaller"/> class.
		/// </summary>
		/// <param name="connection">SLNet raw connection. Can be obtained via Engine object.</param>
		/// <param name="context">
		/// Provides the context to which the installer has access to. Skyline.DataMiner.Net.AppPackages.AppInstallContext object is automatically provided by the Automation script.
		/// </param>
		/// <param name="logMethod">The app installers log method, used to write to the SLAppPackageInstaller.txt.</param>
		public DomInstaller(IConnection connection, AppInstallContext context, Action<string> logMethod)
		{
			// Null checks still required / valid input data check
			this.context = context;
			this.logMethod = logMethod;
			domImporter = new DomImporter(connection.HandleMessages);
			domImporter.Progress += DomImporter_Progress;
		}

		/// <summary>
		/// Installs all exported DOM files from the DOM editor script in the AppContent of the package on the DMS.
		/// </summary>
		public void InstallDefaultContent()
		{
			Log("Installing DOM items");

			if(!Directory.Exists(context.AppContentPath + DOM_FOLDERPATH))
			{
				Log("No DOM Modules found, skipping this step.");
				return;
			}

			foreach (var exportedDomJson in Directory.GetFiles(context.AppContentPath + DOM_FOLDERPATH, "*.json"))
			{
				Log("Installing " + exportedDomJson);
				domImporter.Import(exportedDomJson);
			}
		}

		/// <summary>
		/// Logs the provided message into the logfile "C:\Skyline DataMiner\Logging\SLAppPackageInstaller.txt".
		/// The message will also be shown during installation with the DataMiner Application
		/// Package Installer from the Skyline TaskBar Utility.
		/// </summary>
		/// <param name="message">A message you want to log or show during installation.</param>
		public void Log(string message)
		{
			logMethod(message);
		}

		private void DomImporter_Progress(object sender, ItemProgressEventArgs e)
		{
			Log("Installed " + e.Items + " DOM item(s)");
		}
	}
}