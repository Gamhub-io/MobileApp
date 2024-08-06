
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace GamHubApp.Helpers
{
    public class EmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty((string)value) ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
