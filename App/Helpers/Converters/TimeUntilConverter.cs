

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
            else if (timeremaining.TotalHours < 1)
            {
                if (timeremaining.Minutes > 1)
                    return $"{timeremaining.Minutes} minutes"; 
                return $"1 minute"; 
            }
            else if (timeremaining.TotalDays < 1)
            {
                string hoursStr = timeremaining.Hours > 1? $"{timeremaining.Hours} hours": "1 hour";
                string minutesStr = timeremaining.Minutes > 1? $"{timeremaining.Minutes} minutes" : "1 minute";

                return $"{hoursStr} {minutesStr}";
            }
            else if (timeremaining.TotalDays < 30)
            {
                string hoursStr = timeremaining.Hours > 1 ? $"{timeremaining.Hours} hours" : "1 hour";
                string daysStr = timeremaining.Days > 1 ? $"{timeremaining.Days} days" : "1 day";

                return $"{daysStr} {hoursStr}";
            }


        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
