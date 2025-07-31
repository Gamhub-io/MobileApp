using GamHubApp.Models;
using GamHubApp.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class GiveawayWinsViewModel(Fetcher fetc) : BaseViewModel
{
    public Fetcher DataFetcher { get; } = fetc;
    private ObservableCollection<Giveaway> _wins;

    public ObservableCollection<Giveaway> Wins
    {
        get
        {
            return _wins;
        }
        set
        {
            _wins = value;
            OnPropertyChanged(nameof(Wins));
        }
    }
#if IOS
    public async Task GetWins()
    {
        Wins = new (await DataFetcher.GetWonGiveaways());
    }
#endif
}
