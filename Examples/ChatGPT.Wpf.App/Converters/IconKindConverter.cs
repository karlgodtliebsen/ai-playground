using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace ChatGPT.Wpf.App.Converters;
public class IconKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string x = (string)value;
        if (x == "A")
        {
            return PackIconKind.QuestionAnswer;
        }
        else if (x == "Q")
        {
            return PackIconKind.CommentQuestion;
        }
        else if (x == "F")
        {
            return PackIconKind.EmoticonSad;
        }
        return PackIconKind.EmoticonNeutral;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
