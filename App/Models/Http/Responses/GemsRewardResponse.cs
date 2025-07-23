
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public class GemsRewardResponse
{
    [JsonProperty("data")]
    public List<string> Data { get; set; }
    [JsonProperty("rewarded")]
    public bool Rewarded { get; set; }
}
