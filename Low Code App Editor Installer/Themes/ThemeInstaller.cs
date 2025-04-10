namespace Install_1.Themes
{
    using System;
    using System.IO;
    using System.Linq;
    using Install_1;

    using Newtonsoft.Json.Linq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.AppPackages;

    public class ThemeInstaller
	{
		private const string Themes_Context_PATH = @"CompanionFiles\Themes\Themes.json";
		private const string Themes_DataMiner_Path = @"C:\Skyline DataMiner\dashboards\Themes.json";

		private readonly IConnection connection;
		private readonly AppInstallContext context;
		private readonly InstallOptions options;
		private readonly Action<string> logMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThemeInstaller"/> class.
		/// </summary>
		/// <param name="connection">SLNet raw connection. Can be obtained via Engine object.</param>
		/// <param name="context">
		/// Provides the context to which the installer has access to. Skyline.DataMiner.Net.AppPackages.AppInstallContext object is automatically provided by the Automation script.
		/// </param>
		/// <param name="options">The install options coming from the Low Code App Editor script.</param>
		/// <param name="logMethod">The app installers log method, used to write to the SLAppPackageInstaller.txt.</param>
		public ThemeInstaller(IConnection connection, AppInstallContext context, InstallOptions options, Action<string> logMethod)
		{
			this.connection = connection;
			this.context = context;
			this.options = options;
			this.logMethod = logMethod;
		}

		/// <summary>
		/// Installs all provided images in the AppContent of the package on the DMS.
		/// </summary>
		public void InstallDefaultContent()
		{
			Log("Low Code App Themes Installation");

			if (!File.Exists(context.AppContentPath + Themes_Context_PATH))
			{
				Log("No Themes found, skipping this step.");
				return;
			}

			var appThemesContent = File.ReadAllText(context.AppContentPath + Themes_Context_PATH);
			var appThemes = JObject.Parse(appThemesContent)["Themes"] as JArray;

			if (appThemes.Any())
			{
				Log($"Found {appThemes.Count} theme(s) to install.");
			}
			else
			{
				Log("No themes found, skipping this step.");
				return;
			}

			// Check if themes file exists
			if (!File.Exists(Themes_DataMiner_Path))
			{
				Log("C:\\Skyline DataMiner\\dashboards\\Themes.json file does not exists, creating it.");
				File.WriteAllText(Themes_DataMiner_Path, appThemesContent);
				if (options.SyncThemes)
				{
					connection.SyncFile(Themes_DataMiner_Path, Enums.FileSyncType.Added);
				}

				return;
			}

			Log("Parsing C:\\Skyline DataMiner\\dashboards\\Themes.json");
			var allThemesContent = File.ReadAllText(Themes_DataMiner_Path);
			var allThemesFull = JObject.Parse(allThemesContent);
			var allThemes = allThemesFull["Themes"] as JArray;
			var allThemeIds = allThemes.Select(t => t["ID"].Value<int>()).ToList();

			for (int i = 0; i < appThemes.Count; i++)
			{
				var theme = appThemes[i];
				var appThemeId = theme["ID"].Value<int>();
				var appThemeName = theme["Name"].Value<string>();

				var existingTheme = allThemes.FirstOrDefault(t => t["Name"].Value<string>() == appThemeName);
				if (existingTheme != null)
				{
					if (!options.OverwriteThemes)
					{
						continue;
					}

					var existingThemeId = existingTheme["ID"].Value<int>();
					theme["ID"] = existingThemeId;

					var existingThemeIndex = allThemes.IndexOf(existingTheme);
					allThemes[existingThemeIndex] = theme;
				}
				else
				{
					if (allThemeIds.Exists(x => x == appThemeId))
					{
						var newId = allThemeIds.Max() + 1;
						theme["ID"] = newId;
						appThemeId = newId;
						allThemeIds.Add(newId);
					}

					allThemes.Add(theme);
				}

				Log($"[{i + 1}/{appThemes.Count}] Installed '{appThemeName}'");
			}

			File.WriteAllText(Themes_DataMiner_Path, allThemesFull.ToString());
			if (options.SyncThemes)
			{
				connection.SyncFile(Themes_DataMiner_Path, Enums.FileSyncType.Changed);
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
	}
}