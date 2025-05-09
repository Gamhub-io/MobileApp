﻿using GamHubApp.Models;
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

    public DealsViewModel()
    {
        CurrentApp = App.Current as App;
    }

    public async Task UpdateDeals()
    {
        Deals = new ((await CurrentApp.DataFetcher.GetDeals()).OrderBy(d => d.Expires));

        CurrentApp.RemoveLoadingIndicator();
    }
}
