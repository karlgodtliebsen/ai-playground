using System;
using System.Globalization;
using System.Windows.Data;
using OpenAI.Client.OpenAI.Models.Images;

namespace ChatGPT.Wpf.App.Converters;

public class ResponseFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null) return ((ImageResponseFormat)value!).ToString();
        return ImageResponseFormat.Url.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var v = value?.ToString();
        if (v is not null)
        {
            if (v == ImageResponseFormat.B64Json.ToString())
            {
                return ImageResponseFormat.B64Json;
            }
            if (v == ImageResponseFormat.Url.ToString())
            {
                return ImageResponseFormat.Url;
            }
        }
        return ImageResponseFormat.Url.ToString();

    }
}