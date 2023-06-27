using System.Collections.ObjectModel;
using System.ComponentModel;

using ChatGPTClient.Dialogs.Models;
using ChatGPTClient.Models;

namespace ChatGPTClient.TabPages;

public class CreateImageViewModel : INotifyPropertyChanged
{

    public ObservableCollection<RichResultViewModel> Question { get; } = new() { };

    public GenerateImageOptionsViewModel Options { get; } = new();

    public ImagesViewModel Result { get; set; } = new();
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
