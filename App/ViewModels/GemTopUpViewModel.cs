
using GamHubApp.Core;
using GamHubApp.Models;
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
            var cur = App.Current as App;
            if (_selectedPlan is null)
                return;

            cur.ShowLoadingIndicator();
            await _revenueCatBilling.PurchaseProduct(_selectedPlan.Package).ConfigureAwait(false);
            _ = await cur.DataFetcher.UserGemsSync().ConfigureAwait(false);
            cur.RemoveLoadingIndicator();
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
