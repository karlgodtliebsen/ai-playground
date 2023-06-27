using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChatGPTClient.Converters;

public class KindToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string x = (string)value;
        if (x == "A")
        {
            return new Visibility();
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
