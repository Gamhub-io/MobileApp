using Newtonsoft.Json;

namespace GamHubApp.Models;

public class Deal
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("createdAt")]
    public string Created { get; set; }
    [JsonProperty("expiresAt")]
    public string Expires { get; set; }
    [JsonProperty("partner")]
    public Partner Partner { get; set; }
}
