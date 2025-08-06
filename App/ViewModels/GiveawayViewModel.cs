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
    private ObservableCollection<Giveaway> _entries;
    public ObservableCollection<Giveaway> Entries
    {
        get 
        {
            return _entries; 
        }
        set
        {
            _entries = value;
            OnPropertyChanged(nameof(Entries));
        }
    }
    public GiveawayViewModel(Fetcher fetch)
    {
        _fetcher = fetch;
#if IOS
        RefreshGiveawayList().GetAwaiter();
#endif
        
    }
#if IOS

    /// <summary>
    /// Update the Giveaway list
    /// </summary>
    public async Task RefreshGiveawayList()
    {
        Task<List<Giveaway>> giveawayTask = _fetcher.GetGiveaways();
        Task<List<Giveaway>> entriesTask = _fetcher.GetEnteredGiveaways();

        await Task.WhenAll(giveawayTask, entriesTask);    
        Giveaways = new (await giveawayTask);
        Entries = new(await entriesTask);

        for (int i = 0; i< _entries.Count; i++)
        {
            var giveaway = _giveaways.Single(ga => ga.Id == _entries[i].Id);

            if (giveaway?.IsEntered == false)
                Giveaways[_giveaways.IndexOf(giveaway)].IsEntered = true;
        }
    }
#endif
}
