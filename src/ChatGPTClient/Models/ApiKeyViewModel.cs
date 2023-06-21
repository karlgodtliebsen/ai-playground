using System.ComponentModel;

namespace ChatGPTClient.Models;

public sealed class ApiKeyViewModel : INotifyPropertyChanged
{
    private string apiKey = "";

    public string ApiKey
    {
        get { return apiKey; }
        set
        {
            if (apiKey != value)
            {
                apiKey = value;
                OnPropertyChanged(nameof(ApiKey));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
