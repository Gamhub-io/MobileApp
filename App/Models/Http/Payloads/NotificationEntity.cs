
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Payloads;

public class NotificationEntity
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("token")]
    public string Token { get; set; }
}
