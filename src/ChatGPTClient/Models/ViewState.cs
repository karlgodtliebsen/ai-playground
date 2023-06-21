using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChatGPTClient.Models;

public sealed class ViewState : INotifyPropertyChanged
{
    private Model? selectedModel = default;

    public Model SelectedModel
    {
        get { return selectedModel!; }
        set
        {
            selectedModel = value;
            OnPropertyChanged(nameof(SelectedModel));
        }
    }

    private ObservableCollection<Model> models = new();

    public ObservableCollection<Model> Models
    {
        get { return models; }
        set
        {
            models = value;
            OnPropertyChanged(nameof(Models));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
