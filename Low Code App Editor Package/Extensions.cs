// Ignore Spelling: App

namespace Low_Code_App_Editor_Package
{
	using System;
	using System.Diagnostics;
	using System.IO;

	using Skyline.AppInstaller;
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.Advanced;

	internal static class Extensions
	{
		public static void SyncFile(this IConnection connection, string filePath, FileSyncType fileSyncType, Action<string> logger = null)
		{
			SetDataMinerInfoMessage message = new SetDataMinerInfoMessage
			{
				What = 41,
				StrInfo1 = filePath,
				IInfo2 = (int)fileSyncType,
			};

			var response = connection.HandleSingleResponseMessage(message);
			if (response == null)
			{
				logger?.Invoke($"Could not sync file, did not receive a response. Path: {filePath}");
			}

			if (response is CreateProtocolFileResponse createProtocolFileResponse && createProtocolFileResponse.ErrorCode != 0)
			{
				logger?.Invoke($"Could not sync file, the returned error code was {createProtocolFileResponse.ErrorCode}. Path: {filePath}");
			}
		}

		public static void CreateSymbolicLink(this IConnection connection, string path, string targetPath, Action<string> logger = null)
		{
			var command = $"mklink \"{path}\" \"{targetPath}\"";
			if (!File.Exists(path))
			{
				ExecuteCommand(command, logger);
				connection.SyncFile(path, FileSyncType.Added);
				logger?.Invoke($"Created the symbolic link.");
				return;
			}

			var attributes = File.GetAttributes(path);
			if ((attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
			{
				logger?.Invoke($"Could not create the symbolic link, there is already a file named {Path.GetFileName(path)} in the {Path.GetDirectoryName(path)} folder.");
			}
			else
			{
				logger?.Invoke($"Symbolic link is already present in '{path}', skipping this step.");
			}
		}

		private static void ExecuteCommand(string command, Action<string> logger = null)
		{
			ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
			{
				CreateNoWindow = true, // Hides the console window
				UseShellExecute = false, // Necessary for redirecting output
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				Verb = "runas", // Runs the process as administrator (needed for mklink)
			};

			using (Process process = Process.Start(processInfo))
			{
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (!string.IsNullOrEmpty(output))
				{
					logger?.Invoke(output);
				}

				if (!string.IsNullOrEmpty(error))
				{
					logger?.Invoke(error);
					throw new InvalidOperationException(error);
				}
			}
		}
	}
}
