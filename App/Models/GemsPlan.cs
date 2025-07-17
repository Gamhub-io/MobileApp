

using Maui.RevenueCat.InAppBilling.Models;
using System.Globalization;

namespace GamHubApp.Models;

public class GemsPlan: SelectableModel
{
    public int Gems { get; set; }
    public string PriceDisplay { get; set; }
    public decimal Price { get; set; }
    public PackageDto Package { get; set; }
    public int DefaultDiscount { get
        {
            var value = Convert.ToInt16(Gems * 0.11);
            return (int)Math.Round((double)(100 * (value - Price)) / value);
        } }
    public string OriginalPrice { get
    {

        return $"{GetCurrencySymbol(Package.Product.Pricing.CurrencyCode)}{Gems * 0.11}";
        } }
    public static string GetCurrencySymbol(string currencyCode)
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        for (int i=0; i< cultures.Count(); i++)
        {
            RegionInfo region = new RegionInfo(cultures[i].Name);

            if (region.ISOCurrencySymbol.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return region.CurrencySymbol;
            }
        }

        return "¤";
    }
}
