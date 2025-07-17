

namespace GamHubApp.Models;

public class GemsPlan: SelectableModel
{
    public int Gems { get; set; }
    public string PriceDisplay { get; set; }
    public decimal Price { get; set; }
    public int DefaultDiscount { get
        {
            var value = Convert.ToInt16(Gems * 0.11);
            return (Convert.ToInt16(Math.Round(Price)) - value ) * 100 / value;

        } }
}
