namespace Install_1.DOM
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class DomEditorException : Exception
	{
		public DomEditorException()
		{
		}

		public DomEditorException(string message) : base(message)
		{
		}

		public DomEditorException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DomEditorException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}