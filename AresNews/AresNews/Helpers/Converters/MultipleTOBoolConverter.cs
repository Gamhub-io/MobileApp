using AresNews.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AresNews.Helpers
{
    /// <summary>
    /// Returns true if the collection is >= 2
    /// </summary>
    internal class MultipleTOBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ObservableCollection<Article>;
            return collection.Count > 1;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
