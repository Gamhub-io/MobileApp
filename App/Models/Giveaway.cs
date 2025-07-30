#if IOS
using GamHubApp.Views;
#endif
using Newtonsoft.Json;

namespace GamHubApp.Models;

public class Giveaway : SelectableModel
{

    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("key")]
    public string Key { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("entryCost")]
    public int EntryCost { get; set; }
    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }
    [JsonProperty("drm")]
    public GamePlatform DRM { get; set; }
    public Command EnterCommand
    {
#if IOS
        get => new(() =>
        {
            if (IsEntered)
                return;
            (App.Current as App).OpenPopUp(new TwoChoiceQuestionPopUp("Enter giveway", $"Would you like to enter this giveway? cost {EntryCost} Gems", primaryChoice: "Yes", secondaryChoice: "No", async () =>
            {
                await (App.Current as App).DataFetcher.EnterGiveaways(this);

            }));
        });
#else
        get;
#endif
    }

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
