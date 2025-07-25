

using Maui.RevenueCat.InAppBilling.Models;

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
    public string OriginalPrice 
    { 
        get
        {
            return (Gems * 0.11).ToString("C");
        } 
    }
}
