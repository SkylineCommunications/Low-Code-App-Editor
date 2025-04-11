namespace Low_Code_App_Editor.LCA
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Serializable]
    public class AppSettings
    {
        [JsonProperty("PublicVersion")]
        public int PublicVersion { get; set; }

        [JsonProperty("DraftVersion")]
        public int DraftVersion { get; set; }

        [JsonProperty("Sections")]
        public List<string> Sections { get; set; }
    }
}
