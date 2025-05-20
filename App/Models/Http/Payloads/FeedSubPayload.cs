using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Payloads;

public sealed class FeedSubPayload
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("feed")]
    public string Feed { get; set; }
}
