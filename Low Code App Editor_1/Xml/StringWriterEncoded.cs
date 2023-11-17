namespace Low_Code_App_Editor_1.Xml
{
	using System.IO;
	using System.Text;

	/// <summary>
	/// A StringWriter where you can specify the encoding.
	/// </summary>
	public sealed class StringWriterEncoded : StringWriter
	{
		private readonly Encoding encoding;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringWriterEncoded"/> class, with UTF-8 as encoding.
		/// </summary>
		public StringWriterEncoded() : this(Encoding.UTF8) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="StringWriterEncoded"/> class, with the specified encoding.
		/// </summary>
		/// <param name="encoding"></param>
		public StringWriterEncoded(Encoding encoding)
		{
			this.encoding = encoding;
		}

		/// <inheritdoc/>
		public override Encoding Encoding => encoding;
	}
}
