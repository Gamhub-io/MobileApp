
using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Views;
using Maui.RevenueCat.InAppBilling.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class GemTopUpViewModel : BaseViewModel
{
    private ObservableCollection<GemsPlan> _plans;
    public ObservableCollection<GemsPlan> Plans
    {
        get
        {
            return _plans;
        }
        set
        {
            _plans = value;
            OnPropertyChanged(nameof(Plans));
        }
    }
    private bool _planSelected;
    public bool PlanSelected
    {
        get
        {
            return _planSelected;
        }
        set
        {
            _planSelected = value;
            OnPropertyChanged(nameof(PlanSelected));
        }
    }

    private string _gemAmount;
    public string GemAmount
    {
        get
        {
            return _gemAmount;
        }
        set
        {
            _gemAmount = value;
            OnPropertyChanged(nameof(GemAmount));
        }
    }

    private GemsPlan _selectedPlan;
    public GemsPlan SelectedPlan
    {
        get
        {
            return _selectedPlan;
        }
        set
        {
            _selectedPlan = value;
            PlanSelected = _selectedPlan != null;
            for (int i = 0; i < _plans.Count; i++) 
            {
                if (SelectedPlan.Package.Identifier == _plans[i].Package.Identifier)
                {
                    Plans[i].IsSelected = true;
                    continue;
                }
                Plans[i].IsSelected = false;
            }
            OnPropertyChanged(nameof(SelectedPlan));
        }
    }

    public Command PurchaseCommand { get; }

    private readonly IRevenueCatBilling _revenueCatBilling;

    public GemTopUpViewModel (IRevenueCatBilling revenueCatBilling)
    {
        _revenueCatBilling = revenueCatBilling;
        PurchaseCommand = new(async () =>
        {
            if (_selectedPlan is null)
                return;
            var cur = App.Current as App;
            cur.ShowLoadingIndicator();
            
            //
            // Purchase the gems
            await _revenueCatBilling.PurchaseProduct(_selectedPlan.Package).ContinueWith(async (res) =>
            {
                if (!res.Result.IsSuccess)
                    return;
                // sync the gems to the user (is logged in)
                await cur.DataFetcher.UserGemsSync();

                GemAmount = _selectedPlan.Package.Identifier.Split('_')[0];
                (App.Current as App).OpenPopUp(new PurchaseGemsPopUp(this), ((App.Current as App).Windows[0].Page as AppShell).CurrentPage);
            }).ContinueWith(async (_) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            cur.RemoveLoadingIndicator();
            }).ConfigureAwait(false);


        });
    }
    public async Task LoadOptionsAsync()
    {

        var loadedOfferings = await _revenueCatBilling.GetOfferings(true);
        if (!_revenueCatBilling.IsInitialized())
            return;
        await _revenueCatBilling.Login(await SecureStorage.Default.GetAsync(AppConstant.InstanceIdKey));
        Plans = new(loadedOfferings
            .SelectMany(x => x.AvailablePackages
            .Where(x => x.OfferingIdentifier == "gems_top_ups")
            .Select(p => new GemsPlan
            {
                Gems = Convert.ToInt16(p.Identifier.Split('_')[0]),
                PriceDisplay = p.Product.Pricing.PriceLocalized,
                Price = p.Product.Pricing.Price,
                Package = p

            })).OrderBy(l => l.Gems)
            );
    }
}
