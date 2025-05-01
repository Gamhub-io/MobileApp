
using GamHubApp.Models;

namespace GamHubApp.Helpers.Comparers;

public class DealComparer : IEqualityComparer<Deal>
{
    bool IEqualityComparer<Deal>.Equals(Deal x, Deal y)
    {
        return x.Id == y.Id && x.Url == y.Url;
    }

    int IEqualityComparer<Deal>.GetHashCode(Deal deal)
    {
        if (Object.ReferenceEquals(deal, null)) return 0;

        // Replace "PropertyName" with the name of the property you want to compare
        int hashProductName = deal.Id == null ? 0 : deal.Id.GetHashCode();

        return hashProductName;
    }
}
