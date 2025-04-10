// Ignore Spelling: LCA app apps logfile

namespace Install_1.LCA
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Install_1;
    using Install_1.Enums;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.AppPackages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

    public class LcaInstaller
	{
		private const string LCA_FOLDERPATH = @"CompanionFiles\LCA";
		private const string ApplicationsDirectory = @"C:\Skyline DataMiner\applications";

		private readonly IConnection connection;
		private readonly AppInstallContext context;
		private readonly InstallOptions options;
		private readonly Action<string> logMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="LcaInstaller"/> class.
		/// </summary>
		/// <param name="connection">SLNet raw connection. Can be obtained via Engine object.</param>
		/// <param name="context">
		/// Provides the context to which the installer has access to. Skyline.DataMiner.Net.AppPackages.AppInstallContext object is automatically provided by the Automation script.
		/// </param>
		/// <param name="options">The install options coming from the Low Code App Editor script.</param>
		/// <param name="logMethod">The app installers log method, used to write to the SLAppPackageInstaller.txt.</param>
		public LcaInstaller(IConnection connection, AppInstallContext context, InstallOptions options, Action<string> logMethod)
		{
			// Null checks still required / valid input data check
			this.connection = connection;
			this.context = context;
			this.options = options;
			this.logMethod = logMethod;
		}

		/// <summary>
		/// Installs all provided low code apps in the AppContent of the package on the DMS.
		/// </summary>
		public void InstallDefaultContent()
		{
			Log("Low Code App Installation");

			var apps = Directory.GetDirectories(context.AppContentPath + LCA_FOLDERPATH);
			Log($"Installing {apps.Length} app(s)");

			var existingApps = Directory.GetDirectories(ApplicationsDirectory).Select(dir => Path.GetFileName(dir));
			var existingAppsNames = GetAppNames(existingApps);

			for (int i = 0; i < apps.Length; i++)
			{
				var app = apps[i];
				var newName = GetAppName(app);

				if (options.OverwritePreviousVersions)
				{
					CopyFolder(app, Path.Combine(ApplicationsDirectory, Path.GetFileName(app)), true, options.SyncAppToDms);
				}
				else
				{
					newName = SafeInstallApp(existingApps, existingAppsNames, app);
				}

				Log($"[{i + 1}/{apps.Length}] Installed '{newName}'");
			}
		}

		private string SafeInstallApp(IEnumerable<string> existingApps, IEnumerable<string> existingAppsNames, string app)
		{
			// Check if the name of the app already exists
			var appName = GetAppName(app);
			int counter = 0;
			var newName = appName;
			while (existingAppsNames.Contains(newName))
			{
				counter++;
				newName = $"{appName} ({counter})";
			}

			SetAppName(app, newName);

			if (existingApps.Contains(Path.GetFileName(app)))
			{
				// Generate a new GUID for the app
				var oldId = Path.GetFileName(app);
				var newId = Guid.NewGuid().ToString();
				var destPath = Path.Combine(ApplicationsDirectory, newId);
				CopyFolder(app, destPath, false, options.SyncAppToDms);
				foreach (var versionPath in Directory.GetDirectories(destPath))
				{
					var configPath = Path.Combine(versionPath, "App.config.json");
					var file = File.ReadAllText(configPath);
					File.WriteAllText(configPath, file.Replace(oldId, newId));
					if (options.SyncAppToDms)
					{
						connection.SyncFile(file, FileSyncType.Changed);
					}
				}
			}
			else
			{
				CopyFolder(app, Path.Combine(ApplicationsDirectory, Path.GetFileName(app)), false, options.SyncAppToDms);
			}

			return newName;
		}

		private string[] GetAppNames(IEnumerable<string> existingApps)
		{
			var names = new List<string>();
			foreach (var app in existingApps)
			{
				names.Add(GetAppName(app));
			}

			return names.Where(name => name != null).ToArray();
		}

		private string GetAppName(string appId)
		{
			try
			{
				// Get public version
				var info = System.IO.File.ReadAllText(Path.Combine(ApplicationsDirectory, appId, "App.info.json"));
				var pulicVersion = JObject.Parse(info)["PublicVersion"].Value<int>();

				// If public is not available grab draft version
				if (pulicVersion <= 0)
				{
					pulicVersion = JObject.Parse(info)["DraftVersion"].Value<int>();
				}

				// Get name
				var config = System.IO.File.ReadAllText(Path.Combine(ApplicationsDirectory, appId, $"version_{pulicVersion}", "App.config.json"));
				var name = JObject.Parse(config)["Name"].Value<string>();

				return name;
			}
			catch (Exception ex)
			{
				Log($"Could not get name of app with ID: {appId}\nException: \n{ex}");
				return null;
			}
		}

		private void SetAppName(string appId, string name)
		{
			try
			{
				// Get public version
				var info = System.IO.File.ReadAllText(Path.Combine(ApplicationsDirectory, appId, "App.info.json"));
				var pulicVersion = JObject.Parse(info)["PublicVersion"].Value<int>();

				// set name
				var config = System.IO.File.ReadAllText(Path.Combine(ApplicationsDirectory, appId, $"version_{pulicVersion}", "App.config.json"));
				var json = JObject.Parse(config);
				json["Name"] = name;
				System.IO.File.WriteAllText(Path.Combine(ApplicationsDirectory, appId, $"version_{pulicVersion}", "App.config.json"), json.ToString());
			}
			catch (Exception)
			{
				Log($"Could not set name of app with ID: {appId}, to {name}");
			}
		}

		/// <summary>
		/// Copy a folder recursively to the destination folder.
		/// </summary>
		/// <param name="sourceFolderPath">The source folder.</param>
		/// <param name="destinationFolderPath">The destination folder.</param>
		/// <param name="overwrite"><see langword="true"/> will overwrite existing files. <see langword="false"/> to leave the existing files.</param>
		/// /// <param name="syncFiles"><see langword="true"/> will send a sync the files to the other dma's in the dms. <see langword="false"/> to not sync files.</param>
		private void CopyFolder(string sourceFolderPath, string destinationFolderPath, bool overwrite = false, bool syncFiles = false)
		{
			DirectoryInfo sourceDir = new DirectoryInfo(sourceFolderPath);

			// If the destination folder doesn't exist, create it
			if (!Directory.Exists(destinationFolderPath))
			{
				Directory.CreateDirectory(destinationFolderPath);
			}

			// Copy files in the current directory
			foreach (FileInfo file in sourceDir.GetFiles())
			{
				var destFile = Path.Combine(destinationFolderPath, file.Name);
				var syncType = FileSyncType.Changed;
				if (!File.Exists(destFile))
				{
					syncType = FileSyncType.Added;
				}

				if(File.Exists(destFile) && !overwrite)
				{
					// If the file exist and overwrite is not true, leave the file be.
					continue;
				}

				file.CopyTo(destFile, overwrite);

				if (syncFiles)
				{
					connection.SyncFile(destFile, syncType);
				}
			}

			// Recursively copy subdirectories
			foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
			{
				string destSubdir = Path.Combine(destinationFolderPath, subdir.Name);
				CopyFolder(subdir.FullName, destSubdir, overwrite, syncFiles);
			}
		}

		/// <summary>
		/// Logs the provided message into the logfile "C:\Skyline DataMiner\Logging\SLAppPackageInstaller.txt".
		/// The message will also be shown during installation with the DataMiner Application
		/// Package Installer from the Skyline TaskBar Utility.
		/// </summary>
		/// <param name="message">A message you want to log or show during installation.</param>
		private void Log(string message)
		{
			logMethod(message);
		}
	}
}