using GamHubApp.Models.Http.Payloads;
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;
public sealed class NeResponse
{
    
    [JsonProperty("response")]
    public string Response { get; set; }
    [JsonProperty("success")]
    public bool Success { get; set; }
    [JsonProperty("data")]
    public NotificationEntity Data { get; set; }

}
