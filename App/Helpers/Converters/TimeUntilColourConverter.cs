

using System.Globalization;

namespace GamHubApp.Helpers;

public class TimeUntilColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime enddate) 
        {
            var timeremaining = enddate - DateTime.Now;

            if (timeremaining.TotalDays < 5)
                return timeremaining.TotalHours < 20? App.Current.Resources["DealExpiringColor"]: App.Current.Resources["DealExpiresVerySoonColor"];
            else if (timeremaining.TotalDays < 30)
                return App.Current.Resources["DealExpiresSoonColor"];

        }
        return App.Current.Resources["FontColor"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
