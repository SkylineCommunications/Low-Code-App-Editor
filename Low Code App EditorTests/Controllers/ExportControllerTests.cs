// Ignore Spelling: App

namespace Low_Code_App_Editor.Controllers.Tests
{
	using System.IO;
	using System.Linq;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Newtonsoft.Json.Linq;

	[TestClass]
	public class ExportControllerTests
	{
		[TestMethod]
		public void ExtractThemeFromFile()
		{
			// Original themes
			var themesJson = JObject.Parse(File.ReadAllText(Path.Combine("CompanionFiles", "Themes.json")));
			var allThemes = themesJson["Themes"] as JArray;

			// Create a copy of the themes object, clear all the themes out.
			var usedThemesJson = themesJson.DeepClone();
			var usedThemesArray = usedThemesJson["Themes"] as JArray;
			usedThemesArray.Clear();

			// Add the theme called "EXTRACT ME PLEASE"
			var usedTheme = allThemes.First(t => t["Name"].Value<string>() == "EXTRACT ME PLEASE");
			usedThemesArray.Add(usedTheme);

			var correct = JObject.Parse(File.ReadAllText(Path.Combine("CompanionFiles", "Themes_Extracted.json")));

			Assert.IsTrue(JToken.DeepEquals(usedThemesJson, correct));
		}

		[TestMethod]
		public void MergeThemeFromFile()
		{
			// Original themes
			var themesJson = JObject.Parse(File.ReadAllText(Path.Combine("CompanionFiles", "Themes_Merged.json")));
			var allThemes = themesJson["Themes"] as JArray;
			var allThemeIds = allThemes.Select(t => t["ID"].Value<int>()).ToList();

			// Themes to be imported
			var appThemesJson = JObject.Parse(File.ReadAllText(Path.Combine("CompanionFiles", "Themes_Extracted.json")));
			var appThemes = appThemesJson["Themes"] as JArray;

			// Merge the themes
			for (int i = 0; i < appThemes.Count; i++)
			{
				var theme = appThemes[i];
				var appThemeId = theme["ID"].Value<int>();
				var appThemeName = theme["Name"].Value<string>();

				var existingTheme = allThemes.FirstOrDefault(t => t["Name"].Value<string>() == appThemeName);
				if (existingTheme != null)
				{
					var existingThemeId = existingTheme["ID"].Value<int>();
					if (existingThemeId != appThemeId && allThemeIds.Exists(x => x == appThemeId))
					{
						var newId = allThemeIds.Max() + 1;
						theme["ID"] = newId;
						appThemeId = newId;
						allThemeIds.Add(newId);
					}

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
			}

			// Grab the solution
			var correct = JObject.Parse(File.ReadAllText(Path.Combine("CompanionFiles", "Themes.json")));

			Assert.IsTrue(JToken.DeepEquals(themesJson, correct));
		}
	}
}