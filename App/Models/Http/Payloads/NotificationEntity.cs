
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Payloads;

public class NotificationEntity
{
    [JsonProperty("token")]
    public string Token { get; set; }
    [JsonProperty("feed")]
    public string Feed { get; set; }
}
