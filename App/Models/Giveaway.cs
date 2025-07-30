
using Newtonsoft.Json;

namespace GamHubApp.Models;

public class Giveaway
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("key")]
    public string Key{ get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("entryCost")]
    public int EntryCost{ get; set; }
    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }
    [JsonProperty("drm")]
    public GamePlatform DRM { get; set; }
    private bool _isEntered;

    public bool IsEntered 
    { 
        get => _isEntered;
        set
        {
            _isEntered = value;
            OnPropertyChanged(nameof(IsEntered));
        }
    }
}
