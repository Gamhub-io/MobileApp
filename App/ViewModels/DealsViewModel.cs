﻿using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Views;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class DealsViewModel : BaseViewModel
{
    private ObservableCollection<Deal> _deals;
    public App CurrentApp { get; }

    public ObservableCollection<Deal> Deals 
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
        filterCode = Preferences.Get(AppConstant.DealFilterCode, filterCode);
        DealFilterCommand = new Command(() =>
        {
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
            Preferences.Set(AppConstant.DealFilterCode, filterCode = filterCode.TrimEnd());

            Deals = new (CurrentApp.DataFetcher.AllDeals.Where(deal => filterCode.Split('_').Contains(deal.DRM)).OrderBy(d => d.Expires));


            await _lastFilterPopUp?.CloseAsync();
        });

        CancelFilter = new Command(async() =>
        {
            await _lastFilterPopUp?.CloseAsync();
        });

        Task.Run(async () => {
            Platforms = new((await (App.Current as App).DataFetcher.GetDRMs()).OrderBy(plat => plat.DRM));
            for (int i = 0; i < _platforms.Count && filterCode!= null; i++)
            {
                Platforms[i].IsSelected = filterCode.Split('_').Contains(_platforms[i].Id);
            }
        });
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
        Deals = new ((await CurrentApp.DataFetcher.GetDeals()).OrderBy(d => d.Expires));

        CurrentApp.RemoveLoadingIndicator();
    }
}
