using BlenderRenderController.Infra;
using BRClib;
using System;
using Newtonsoft.Json;

namespace BlenderRenderController
{
    [JsonObject(Description = "Brc settings")]
    public class BrcSettings
    {
        [JsonProperty("RecentBlends")]
        public RecentBlendsCollection RecentProjects { get; set; }

        public string BlenderProgram { get; set; }
        public string FFmpegProgram { get; set; }
        public bool DisplayToolTips { get; set; }
        public AfterRenderAction AfterRender { get; set; }
        public Renderer Renderer { get; set; }
        public bool DeleteChunksFolder { get; set; }
        public int LoggingLevel { get; set; }

    }
}
