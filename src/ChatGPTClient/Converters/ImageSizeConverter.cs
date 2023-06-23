using System;
using System.Globalization;
using System.Windows.Data;
using OpenAI.Client.Models;

namespace ChatGPTClient.Converters;

public class ImageSizeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null) return ((ImageSize)value!).ToString();
        return ImageSize.Size1024.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var v = value?.ToString();
        if (v is not null)
        {
            if (v == ImageSize.Size256.ToString())
            {
                return ImageSize.Size256;
            }
            if (v == ImageSize.Size512.ToString())
            {
                return ImageSize.Size512;
            }
            if (v == ImageSize.Size1024.ToString())
            {
                return ImageSize.Size1024;
            }
        }
        return ImageResponseFormat.Url.ToString();

    }
}