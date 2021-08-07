using Newtonsoft.Json;

namespace AresNews.Models
{
    public class Source
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("primaryColour")]
        public string PrimaryColour { get; set; }
        [JsonProperty("secondaryColour")]
        public string SecondaryColour { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }
}
