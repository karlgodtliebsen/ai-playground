using System.ComponentModel;

public class PromptTextModel : INotifyPropertyChanged
{
    private string text = "Enter text";

    public string Text
    {
        get { return text; }
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}