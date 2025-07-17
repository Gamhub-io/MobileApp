
using Newtonsoft.Json;

namespace GamHubApp.Models;

public class Gem
{
    [JsonProperty("_id")]
    public string Id { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; }

    [JsonProperty("instanceID")]
    public string InstanceID { get; set; }

    [JsonProperty("user")]
    public string? User { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

}
