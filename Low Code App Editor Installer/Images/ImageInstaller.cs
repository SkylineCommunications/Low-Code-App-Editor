namespace Install_1.Images
{
    using System;
    using System.IO;
    using Install_1;
    using Install_1.Enums;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.AppPackages;

    public class ImageInstaller
	{
		private const string Images_FOLDERPATH = @"CompanionFiles\Images";
		private const string Images_Directory = @"C:\Skyline DataMiner\Dashboards\_IMAGES";

		private readonly IConnection connection;
		private readonly AppInstallContext context;
		private readonly InstallOptions options;
		private readonly Action<string> logMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageInstaller"/> class.
		/// </summary>
		/// <param name="connection">SLNet raw connection. Can be obtained via Engine object.</param>
		/// <param name="context">
		/// Provides the context to which the installer has access to. Skyline.DataMiner.Net.AppPackages.AppInstallContext object is automatically provided by the Automation script.
		/// </param>
		/// <param name="options">The install options coming from the Low Code App Editor script.</param>
		/// <param name="logMethod">The app installers log method, used to write to the SLAppPackageInstaller.txt.</param>
		public ImageInstaller(IConnection connection, AppInstallContext context, InstallOptions options, Action<string> logMethod)
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
			Log("Low Code App Images Installation");

			if (!Directory.Exists(context.AppContentPath + Images_FOLDERPATH))
			{
				Log("No Images found, skipping this step.");
				return;
			}

			// Check if images folder exists
			if (!Directory.Exists(Images_Directory))
			{
				Log("C:\\Skyline DataMiner\\Dashboards\\_IMAGES directory does not exists, creating it.");
				Directory.CreateDirectory(Images_Directory);
			}

			var images = Directory.GetFiles(context.AppContentPath + Images_FOLDERPATH);
			Log($"Installing {images.Length} image(s).");
			for (int i = 0; i < images.Length; i++)
			{
				var image = images[i];
				var path = Path.Combine(Images_Directory, Path.GetFileName(image));
				FileSyncType syncType;
				if (File.Exists(path))
				{
					if (options.OverwriteImages)
					{
						File.Copy(image, path, true);
						syncType = FileSyncType.Added;
						Log($"[{i + 1}/{images.Length}] Installed {Path.GetFileName(image)}");
					}
					else
					{
						Log($"[{i + 1}/{images.Length}] '{image}' already exists, ignoring this image.");
						continue;
					}
				}
				else
				{
					File.Copy(image, path);
					syncType = FileSyncType.Added;
					Log($"[{i + 1}/{images.Length}] Installed {Path.GetFileName(image)}");
				}

				if (options.SyncImages)
				{
					connection.SyncFile(path, syncType);
				}
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