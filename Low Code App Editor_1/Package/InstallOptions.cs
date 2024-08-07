// Ignore Spelling: App

namespace Low_Code_App_Editor_1.Package
{
	using Newtonsoft.Json;

	public class InstallOptions
	{
		[JsonProperty("Overwrite")]
		public bool OverwritePreviousVersions { get; set; }
	}
}
