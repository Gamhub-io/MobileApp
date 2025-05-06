using System.Globalization;

namespace GamHubApp.Helpers;

public class TimeSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        var time = (TimeSpan)value;

        // If it's more than a year
        if (time.Days > 365)
        {
            // get the number of years
            int nbYears = time.Days / 365;

            return $"{nbYears} years ago";
        }
        // It's more than a month
        if (time.Days > 30)
        {
            // Number of months
            int nbMonth = time.Days / 30;

            if (nbMonth == 1)
                return "1 month ago";
            
            return $"{nbMonth} months ago";
        }
        // It's more than a week
        if (time.Days > 7)
        {
            // Number of weeks
            int nbWeeks = time.Days / 7;

            if (nbWeeks == 1)
                return "1 week ago";
            
            return $"{nbWeeks} weeks ago";

        }
        // It's more than a day
        if (time.Days > 0)
        {
            int days = time.Days;

            if (days == 1)
                return "1 day ago";
            
            return $"{days} days ago";
        }
        // it's more than an hour
        if (time.Hours > 0)
        {
            int hours = time.Hours;

            if (hours == 1)
                return "1 hour ago";
            
            return $"{hours} hours ago";
        }
        if (time.Minutes > 0)
        {
            int minutes = time.Minutes;

            if (minutes == 1)
                return "1 minute ago";

            return $"{minutes} minutes ago";
        }
        if (time.Seconds > 0)
        {
            int seconds = time.Seconds;
            if (seconds == 1)
                return "1 second ago";

            return $"{seconds} Seconds ago";
        }
        return "just now";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
