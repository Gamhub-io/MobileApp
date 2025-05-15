

using System.Globalization;

namespace GamHubApp.Helpers;

public class TimeUntilConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime enddate) 
        {
            var timeremaining = enddate - DateTime.Now;

            if (timeremaining < TimeSpan.Zero)
                return "expired";
            else if (timeremaining < new TimeSpan(days: 30, 0, 0, 0))
                return $"Ends in {timeremaining.Days}d {timeremaining.Days}hrs {timeremaining.Minutes}mins";

        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
