using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

internal class UserGemsResponse
{
    [JsonProperty("data")]
    public List<Gem> Data { get; set; }
}
