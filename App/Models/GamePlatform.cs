
using Newtonsoft.Json;

namespace GamHubApp.Models;

public class GamePlatform : SelectableModel
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("drm")]
    public string DRM{ get; set; }
    [JsonProperty("logo")]
    public string Logo{ get; set; }

    public GamePlatform()
    {
        IsSelected = true;
    }

}
