using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace GamHubApp.Helpers
{
    public class ZeroToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value) > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
