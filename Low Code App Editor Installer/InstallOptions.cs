﻿// Ignore Spelling: LCA App Dms

namespace Install_1
{
    using Newtonsoft.Json;

    public class InstallOptions
    {
        [JsonProperty("Overwrite")]
        public bool OverwritePreviousVersions { get; set; }

        [JsonProperty("sync_app")]
        public bool SyncAppToDms { get; set; }

        [JsonProperty("sync_images")]
        public bool SyncImages { get; set; }

        [JsonProperty("overwrite_images")]
        public bool OverwriteImages { get; set; }

        [JsonProperty("sync_themes")]
        public bool SyncThemes { get; set; }

        [JsonProperty("overwrite_themes")]
        public bool OverwriteThemes { get; set; }
    }
}
