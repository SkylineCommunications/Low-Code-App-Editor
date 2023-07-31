namespace Low_Code_App_Editor_1.LCA
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;
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

        [JsonIgnore]
        public string Path { get; set; }

        public List<string> GetUsedScripts()
        {
            var scripts = new List<string>();

            // Search through GQI queries for custom operators
            DataPool.ForEach(query =>
            {
                scripts.AddRange(FindScriptsInChild(((DMADashboardQueryData)query).Query));
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

        private List<string> FindScriptsInChild(DMAGenericInterfaceQuery query)
        {
            var scripts = new List<string>();
            if(query == null || query.Options == null)
            {
                return scripts;
            }

            foreach(var option in query.Options)
            {
                var script = FindScriptInOption(option);
                if(script != null)
                    scripts.Add(script);
            }

            scripts.AddRange(FindScriptsInChild(query.Child));
            return scripts;
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
            catch(Exception)
            {
                return null;
            }
        }
    }
}
