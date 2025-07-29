using GamHubApp.Models;
using GamHubApp.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class GiveawayViewModel : BaseViewModel
{
    private ObservableCollection<Giveaway> _giveaways;
    private Fetcher _fetcher;

    public ObservableCollection<Giveaway> Giveaways
    {
        get 
        {
            return _giveaways; 
        }
        set
        {
            _giveaways = value;
            OnPropertyChanged(nameof(Giveaways));
        }
    }
    public GiveawayViewModel(Fetcher fetch)
    {
        _fetcher = fetch;
        RefreshGiveawayList().GetAwaiter();
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task RefreshGiveawayList()
    {
        Giveaways = new (await _fetcher.GetGiveaways());
    }
}
