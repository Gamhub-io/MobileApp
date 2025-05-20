
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public sealed class SubStatusRes
{
    [JsonProperty("message")]
    public string msg { get; set; }
    [JsonProperty("Enabled")]
    public bool Enabled { get; set; }
}
