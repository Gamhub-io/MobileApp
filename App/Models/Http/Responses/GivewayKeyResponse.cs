
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public class GivewayKeyResponse
{
    [JsonProperty("data")]
    public string Data { get; set; }
    [JsonProperty("response")]
    public string Res { get; set; }

}
