using System.ComponentModel;

public class ApiKeyViewModel : INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
