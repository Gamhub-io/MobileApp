using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Views;
using MvvmHelpers;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class DealsViewModel : BaseViewModel
{
    public App CurrentApp { get; }
    private ObservableRangeCollection<Deal> _deals = new ();

    public ObservableRangeCollection<Deal> Deals
    {
        get
        {
            return _deals;
        }
        set
        {
            _deals = value;
            OnPropertyChanged(nameof(Deals));
        }
    }
    public Command DealFilterCommand { get; }
    public Command SaveFilter { get; }
    public Command CancelFilter { get; }
    private string filterCode = null;

    public DealsViewModel()
    {
        CurrentApp = App.Current as App;
        filterCode = Preferences.Get(PreferencesKeys.DealFilterCode, filterCode);
        DealFilterCommand = new Command(() =>
        {
            if (_filterOpened) return;
            CurrentApp.OpenPopUp(_lastFilterPopUp = new DealFilterPopUp(this));
        });

        SaveFilter = new Command(async () =>
        {
            filterCode = string.Empty;
            List<string> drmIDs = new ();
            foreach (var drm in _platforms)
            {
                if (drm.IsSelected)
                {
                    filterCode += drm.Id + '_';
                    drmIDs.Add(drm.Id);
                }
            }
            Preferences.Set(PreferencesKeys.DealFilterCode, filterCode = filterCode.TrimEnd());
                if (CurrentApp.DataFetcher.AllDeals == null)
                {
                    await App.DisplaySoftError("Sorry, the filters cannot be applied");
                    return;
                }

                var allowedDrms = filterCode.Split('_');
            var filteredDeals = new ObservableRangeCollection<Deal>(
                    CurrentApp.DataFetcher.AllDeals.Where(deal => deal?.DRM != null && allowedDrms.Contains(deal.DRM)).OrderBy(d => d.Expires));

            if (filteredDeals.Any())
                Deals = filteredDeals;

            await _lastFilterPopUp?.CloseAsync();
            _filterOpened = false;
        });

        CancelFilter = new Command(async() =>
        {
            await _lastFilterPopUp?.CloseAsync();
            _filterOpened = false;
        });

        Task.Run(async () => {
            Platforms = new((await (App.Current as App).DataFetcher.GetDRMs()).OrderBy(plat => plat.DRM));
            for (int i = 0; i < _platforms.Count && filterCode!= null; i++)
            {
                Platforms[i].IsSelected = filterCode.Split('_').Contains(_platforms[i].Id);
            }
        }).GetAwaiter();
    }
    
    private DealFilterPopUp _lastFilterPopUp;
    private ObservableCollection<GamePlatform> _platforms;

    public ObservableCollection<GamePlatform> Platforms
    {
        get => _platforms;
        set
        {
            _platforms = value;
            OnPropertyChanged(nameof(Platforms));
        }

    }

    public async Task UpdateDeals()
    {
        if (!(Deals?.Count > 0))
        {
            var getTendingTask = CurrentApp.DataFetcher.GetTrendingDeals();
            var getDealTask = CurrentApp.DataFetcher.GetDeals();

            await foreach (var task in Task.WhenEach(getTendingTask,
                getDealTask)) 
            {

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (task == getTendingTask ) 
                    {
                        var res = getTendingTask.Result
                                    .OrderBy(d => d?.Expires).ToList();
                        if (Deals.Count > 0)
                        {
                            var tendingDeals = new ObservableRangeCollection<Deal>(res);
                            for (int i = 0; i < tendingDeals.Count; i++)
                            {

                                Deals.Insert(i, tendingDeals[i]);
                            }

                        }
                        else
                        {
                            Deals.AddRange(new ObservableRangeCollection<Deal>(res));
                        }
                    }
                    if (task == getDealTask)
                        Deals.AddRange(new ObservableRangeCollection<Deal>(getDealTask.Result.OrderBy(d => d.Expires)));

                });
            }


            return;
        }
        var newDeals = (await CurrentApp.DataFetcher.GetDeals()).OrderBy(d => d.Expires)
                        .Select((deal, index) => new { Deal = deal, Index = index })
                        .Where((d) => _deals.FirstOrDefault(ogD => ogD.Id == d.Deal.Id) == null)
                        .ToList();

        for (int i = 0; i < newDeals.Count; i++)
        {
            var deal = newDeals[i].Deal;
            if (filterCode != null && !filterCode.Split('_').Contains(deal.DRM))
                continue;
            Deals.Insert(newDeals[i].Index, deal);
        }
        long nbExpires = _deals.LongCount(deal => deal.Expires < DateTime.UtcNow);

        for (int i = 0; i < nbExpires; i++)
        {
            Deals.RemoveAt(i);
        }

}
}
