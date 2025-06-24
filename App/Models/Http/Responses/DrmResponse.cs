
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

class DrmResponse
{
    [JsonProperty("response")]
    public string Response { get; set; }
    [JsonProperty("data")]
    public List<GamePlatform> Data { get; set; }
}
