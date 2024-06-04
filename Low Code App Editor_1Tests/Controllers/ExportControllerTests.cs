namespace Low_Code_App_Editor_1.Controllers.Tests
{
	using System.IO;
	using System.Linq;

	using Low_Code_App_Editor_1.Json;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Web.Common.v1.Dashboards;

	[TestClass]
	public class ExportControllerTests
	{
		[TestMethod]
		public void ExtractThemeFromFile()
		{
			// Original themes
			var themesJson = JObject.Parse(File.ReadAllText(@"CompanionFiles\Themes.json"));
			var allThemes = themesJson["Themes"] as JArray;

			// Create a copy of the themes object, clear all the themes out.
			var usedThemesJson = themesJson.DeepClone();
			var usedThemesArray = usedThemesJson["Themes"] as JArray;
			usedThemesArray.Clear();

			// Add the theme called "EXTRACT ME PLEASE"
			var usedTheme = allThemes.First(t => t["Name"].Value<string>() == "EXTRACT ME PLEASE");
			usedThemesArray.Add(usedTheme);

			var correct = JObject.Parse(File.ReadAllText(@"CompanionFiles\Themes_Extracted.json"));

			Assert.IsTrue(JToken.DeepEquals(usedThemesJson, correct));
		}
	}
}