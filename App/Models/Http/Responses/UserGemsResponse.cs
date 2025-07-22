using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public class UserGemsResponse
{
    [JsonProperty("data")]
    public List<Gem> Data { get; set; }
}
