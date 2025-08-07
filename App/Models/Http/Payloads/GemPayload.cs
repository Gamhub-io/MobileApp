

using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Payloads;

public class GemPayload
{
    [JsonProperty("gems")]
    public string[] Gems { get; set; }
}
