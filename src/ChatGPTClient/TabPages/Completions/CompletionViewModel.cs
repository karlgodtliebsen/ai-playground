using System.ComponentModel;

using ChatGPTClient.Dialogs.Models;
using ChatGPTClient.Models;

namespace ChatGPTClient;

public class CompletionViewModel : INotifyPropertyChanged
{

    public CompletionOptionsViewModel Options { get; } = new();

    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
