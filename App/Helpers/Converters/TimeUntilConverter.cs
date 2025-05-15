

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
            else if (timeremaining.TotalHours < 0)
                return $"{timeremaining.Minutes} minutes";
            else if (timeremaining.TotalDays < 0)
                return $"{timeremaining.Days} hours {timeremaining.Minutes} minutes ";
            else if (timeremaining.TotalDays < 30)
                return $"{timeremaining.Days} days {timeremaining.Hours} hours";

        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
