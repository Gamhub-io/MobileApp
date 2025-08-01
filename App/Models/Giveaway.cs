#if IOS
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Views;
using GamHubApp.Models.Http.Responses;
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
    private string _key { get; set; }
    [JsonProperty("key")]
    public string Key
    {
        get => _key;
        set
        {
            _key = value;
            OnPropertyChanged(nameof(Key));
        }
    }
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

                IsEntered = true;
            }));
        });
#else
        get;
#endif
    }
    public Command ClaimCommand
    {
#if IOS
        get => new(() =>
        {
            if (IsEntered)
                return;
            (App.Current as App).OpenPopUp(new TwoChoiceQuestionPopUp("Claim the game", $"Would you like to claim this game?", primaryChoice: "Yes", secondaryChoice: "No", async () =>
            {

                GivewayKeyResponse res = await (App.Current as App).DataFetcher.GetGiveawayKey(this);
                
                if (res is null)
                    return;

                Key = res.Data;
                if (string.IsNullOrEmpty(res.Data))
                {
                    await (App.Current as App).Windows[0].Page.DisplayAlert("Error", res.Res, "OK");
                }

            }));
        });
#else
        get;
#endif
    }
    public Command CopyKeyCommand
    {
#if IOS
        get => new(async () =>
        {
            await Clipboard.Default.SetTextAsync(Key);
            await Toast.Make("Key copied").Show();
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
    private bool _unavailable;

    public bool Unavailable 
    { 
        get => _unavailable;
        set
        {
            _unavailable = value;
            OnPropertyChanged(nameof(Unavailable));
        }
    }
}
