using System.ComponentModel;

using ChatGPTClient.Models;

namespace ChatGPTClient;



public abstract class BaseModel : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

public abstract class BaseViewModel : BaseModel
{

    public ViewState? ViewState { get; set; }


    private bool isReady = true;
    public bool IsReady
    {
        get => isReady;
        set
        {
            if (value == isReady) return;
            isReady = value;
            OnPropertyChanged(nameof(IsReady));
        }
    }
}
