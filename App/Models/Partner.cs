using Newtonsoft.Json;

namespace GamHubApp.Models;

public class Partner
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("logo")]
    public string Logo { get; set; }


}
