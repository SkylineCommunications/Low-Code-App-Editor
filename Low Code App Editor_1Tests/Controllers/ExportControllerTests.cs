namespace Low_Code_App_Editor_1.Controllers.Tests
{
	using System.IO;
	using Low_Code_App_Editor_1.Json;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Newtonsoft.Json;
	using Skyline.DataMiner.Web.Common.v1.Dashboards;

	[TestClass]
	public class ExportControllerTests
	{
		[TestMethod]
		public void SerializeThemes()
		{
			var themesRaw = File.ReadAllText(@"C:\Skyline DataMiner\dashboards\Themes.json");
			var themes = JsonConvert.DeserializeObject<DMADashboardThemes>(themesRaw);

			var usedThemesFile = JsonConvert.SerializeObject(
				new DMADashboardThemes
				{
					Themes = (DMADashboardTheme[])themes.Themes.Clone(),
				}, new TypeConverter());

			Assert.IsTrue(true);
		}
	}
}