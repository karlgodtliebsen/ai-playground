using System.Collections.ObjectModel;
using System.ComponentModel;

using OpenAI.Client.Models;

namespace ChatGPTClient.Dialogs.Models;

public class GenerateImageOptionsViewModel : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ImageSize imageSize = ImageSize.Size1024;
    private int numberOfImagesToGenerate = 1;
    private ImageResponseFormat imageResponseFormat = ImageResponseFormat.B64Json;

    public int NumberOfImagesToGenerate
    {
        get => numberOfImagesToGenerate;
        set
        {
            if (Equals(value, numberOfImagesToGenerate)) return;
            numberOfImagesToGenerate = value;
            OnPropertyChanged(nameof(NumberOfImagesToGenerate));
        }
    }

    public ImageSize ImageSize
    {
        get => imageSize;
        set
        {
            if (Equals(value, imageSize)) return;
            imageSize = value;
            OnPropertyChanged(nameof(ImageSize));
        }
    }

    public ImageResponseFormat ImageResponseFormat
    {
        get => imageResponseFormat;
        set
        {
            if (Equals(value, imageResponseFormat)) return;
            imageResponseFormat = value;
            OnPropertyChanged(nameof(ImageResponseFormat));
        }
    }

    public ObservableCollection<string> ImageSizeSource { get; } = new()
    {
        ImageSize.Size256.ToString(),
        ImageSize.Size512.ToString(),
        ImageSize.Size1024.ToString(),
    };


    public ObservableCollection<string> ImageResponseFormatSource { get; } = new()
            {
            ImageResponseFormat.B64Json.ToString(),
            ImageResponseFormat.Url.ToString(),
            };

}
