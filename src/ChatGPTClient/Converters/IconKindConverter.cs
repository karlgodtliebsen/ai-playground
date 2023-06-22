using System;
using System.Globalization;
using System.Windows.Data;

using MaterialDesignThemes.Wpf;

namespace ChatGPTClient.Converters;
public class IconKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string x = (string)value;
        if (x == "A")
        {
            return PackIconKind.Ansible;
        }
        else if (x == "Q")
        {
            return PackIconKind.Quadcopter;
        }
        else if (x == "F")
        {
            return PackIconKind.EmoticonSad;
        }
        // Return a default icon if no match is found
        return PackIconKind.EmoticonNeutral;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
