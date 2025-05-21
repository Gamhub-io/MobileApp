using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public class FeedResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("data")]
    public Feed Data { get; set; }
}
