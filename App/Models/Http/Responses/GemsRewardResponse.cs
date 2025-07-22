
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public class GemsRewardResponse
{
    [JsonProperty("data")]
    public string Data { get; set; }
    [JsonProperty("rewarded")]
    public bool Rewarded { get; set; }
}
