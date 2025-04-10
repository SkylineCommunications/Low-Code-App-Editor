namespace Install_1
{
	using System;

	using Install_1.Enums;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Exceptions;
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
			if(response == null)
			{
				logger?.Invoke($"Could not sync file, did not receive a response. Path: {filePath}");
			}

			if (response is CreateProtocolFileResponse createProtocolFileResponse && createProtocolFileResponse.ErrorCode != 0)
			{
				logger?.Invoke($"Could not sync file, the returned error code was {createProtocolFileResponse.ErrorCode}. Path: {filePath}");
			}
		}
	}
}
