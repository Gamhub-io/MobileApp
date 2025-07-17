
using GamHubApp.Core;
using GamHubApp.Models;
using Maui.RevenueCat.InAppBilling.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class GemTopUpViewModel : BaseViewModel
{
    public ObservableCollection<GemsPlan> Plans { get; set; }
    private readonly IRevenueCatBilling _revenueCatBilling;

    public GemTopUpViewModel (IRevenueCatBilling revenueCatBilling)
    {
        _revenueCatBilling = revenueCatBilling;
    }
    public async Task LoadOptionsAsync()
    {

        var loadedOfferings = await _revenueCatBilling.GetOfferings(true);
        if (!_revenueCatBilling.IsInitialized())
            return;
        await _revenueCatBilling.Login(await SecureStorage.GetAsync(AppConstant.InstanceIdKey));
        Plans = new(loadedOfferings
            .SelectMany(x => x.AvailablePackages
            .Where(x => x.OfferingIdentifier == "gems_top_ups")
            .Select(p => new GemsPlan
            {
                Gems = Convert.ToInt16(p.Identifier.Split('_')[0]),
                PriceDisplay = p.Product.Pricing.PriceLocalized,
                Price = p.Product.Pricing.Price,

            }))
            );
    }
}
