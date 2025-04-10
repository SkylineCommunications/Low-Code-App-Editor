// Ignore Spelling: App

namespace Low_Code_App_Editor
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class LowCodeAppEditorException : Exception
	{
		public LowCodeAppEditorException() { }

		public LowCodeAppEditorException(string message) : base(message) { }

		public LowCodeAppEditorException(string message, Exception innerException) : base(message, innerException) { }

		protected LowCodeAppEditorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
