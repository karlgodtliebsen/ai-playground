using System.ComponentModel;

namespace ChatGPTClient.TabPages;

public sealed class ImageResultViewModel : INotifyPropertyChanged
{
    private string? imagePath = default!;

    public string? ImagePath
    {
        get { return imagePath; }
        set
        {
            if (imagePath != value)
            {
                imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}