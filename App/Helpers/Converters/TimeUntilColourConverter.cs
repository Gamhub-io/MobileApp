

using System.Globalization;

namespace GamHubApp.Helpers;

public class TimeUntilColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime enddate) 
        {
            var timeremaining = enddate - DateTime.Now;

            if (timeremaining < TimeSpan.Zero)
                return App.Current.Resources["DealExpiringColor"];
            else if (timeremaining.Days < 0)
                return timeremaining.Hours < 20? App.Current.Resources["DealExpiringColor"]: App.Current.Resources["DealExpiresVerySoonColor"];
            else if (timeremaining < new TimeSpan(days: 30, 0, 0, 0))
                return App.Current.Resources["DealExpiresSoonColor"];

        }
        return App.Current.Resources["FontColor"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
