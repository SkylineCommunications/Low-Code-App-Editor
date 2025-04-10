namespace Low_Code_App_Editor.LCA
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Web.Common.v1;
	using Skyline.DataMiner.Web.Common.v1.Dashboards;

	public class AppVersion : DMADynamicApplication
	{
		private static readonly string[] OptionsWithScripts = new[]
		{
			"Operator",
			"Data source",
		};

		private static readonly string[] OptionsWithDomModules = new[]
		{
			"Module",
		};

		[JsonIgnore]
		public string Path { get; set; }

		[JsonIgnore]
		public string FolderPath { get => Directory.GetParent(Path).FullName; }

		public List<string> GetUsedScripts()
		{
			var scripts = new List<string>();

			// Search through GQI queries for custom operators
			DataPool.ForEach(query =>
			{
				if(query is DMADashboardQueryData gqiQuery)
				{
					scripts.AddRange(FindScriptsInChild(gqiQuery.Query));
				}
			});

			// Search through pages for scripts used in actions
			foreach (var page in Pages.Select(page => page.ID))
			{
				var pageFile = System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "pages", $"{page}.dmadb.json"));
				var pageJson = JObject.Parse(pageFile);
				scripts.AddRange(pageJson.FindPropertiesWithName("Script").Select(token => token.Value<string>()));
			}

			return scripts.Distinct().ToList();
		}

		public List<string> GetUsedDomModules()
		{
			var modules = new List<string>();

			// Search through GQI queries for dom modules
			DataPool.ForEach(query =>
			{
				if(query is DMADashboardQueryData gqiQuery)
				{
					modules.AddRange(FindDomModulesInChild(gqiQuery.Query));
				}
			});

			return modules.Distinct().ToList();
		}

		public List<string> GetUsedImages(string basePath = @"C:\Skyline DataMiner\Dashboards\_IMAGES")
		{
			var images = new List<string>();

			// Search through components for themes
			foreach (var pageInfo in Pages)
			{
				var pageRaw = File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "pages", $"{pageInfo.ID}.dmadb.json"));
				var page = JsonConvert.DeserializeObject<DMADashboardConfig>(pageRaw);
				foreach (var component in page.Components.Where(comp => comp.Type == "image"))
				{
					var imageData = JObject.Parse(JsonConvert.SerializeObject(component.InputData));
					images.Add(System.IO.Path.Combine(basePath, imageData.SelectToken("image")["name"].Value<string>()));
				}
			}

			return images;
		}

		public List<DMADashboardTheme> GetUsedThemes(string themesPath = @"C:\Skyline DataMiner\dashboards\Themes.json")
		{
			var themesRaw = File.ReadAllText(themesPath);
			var allThemes = JsonConvert.DeserializeObject<DMADashboardThemes>(themesRaw);

			var themes = new List<DMADashboardTheme>();

			// Search through components for themes
			foreach (var pageInfo in Pages)
			{
				var pageRaw = File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), "pages", $"{pageInfo.ID}.dmadb.json"));
				var page = JsonConvert.DeserializeObject<DMADashboardConfig>(pageRaw);
				var foundTheme = allThemes.Themes.FirstOrDefault(x => x.Name == page.ThemeKey);
				if(foundTheme == null)
				{
					continue;
				}

				themes.Add(foundTheme);
			}

			return themes;
		}

		private List<string> FindScriptsInChild(DMAGenericInterfaceQuery query)
		{
			var scripts = new List<string>();
			if (query == null || query.Options == null)
			{
				return scripts;
			}

			foreach (var option in query.Options)
			{
				var script = FindScriptInOption(option);
				if (script != null)
					scripts.Add(script);
			}

			scripts.AddRange(FindScriptsInChild(query.Child));
			return scripts;
		}

		private List<string> FindDomModulesInChild(DMAGenericInterfaceQuery query)
		{
			var modules = new List<string>();
			if (query == null || query.Options == null)
			{
				return modules;
			}

			foreach (var option in query.Options)
			{
				var module = FindDomModulesInOption(option);
				if (module != null)
					modules.Add(module);
			}

			modules.AddRange(FindDomModulesInChild(query.Child));
			return modules;
		}

		private string FindScriptInOption(DMAGenericInterfaceQueryChosenOption option)
		{
			if (option == null)
			{
				return null;
			}

			if (option.Type != "string")
			{
				return null;
			}

			if (!OptionsWithScripts.Contains(option.ID))
			{
				return null;
			}

			try
			{
				JObject json = JObject.Parse(Convert.ToString(option.Value.Value));
				if (!json.ContainsKey("ScriptName"))
					return null;

				return Convert.ToString(json["ScriptName"]);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private string FindDomModulesInOption(DMAGenericInterfaceQueryChosenOption option)
		{
			if (option == null)
			{
				return null;
			}

			if (option.Type != "string")
			{
				return null;
			}

			if (!OptionsWithDomModules.Contains(option.ID))
			{
				return null;
			}

			try
			{
				var module = Convert.ToString(option.Value.Value);
				return module;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
