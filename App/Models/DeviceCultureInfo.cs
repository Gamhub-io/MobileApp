using Newtonsoft.Json;

namespace GamHubApp.Models;

public class DeviceCultureInfo
{
    [JsonProperty("regionCode")]
    public string RegionCode { get; set; }
}
