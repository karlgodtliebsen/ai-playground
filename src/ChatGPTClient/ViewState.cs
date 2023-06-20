using System.Collections.ObjectModel;
using System.ComponentModel;

public class ViewState : INotifyPropertyChanged
{
    private Model selectedModel = new Model() { ModelId = "text-davinci-003" };

    public ViewState()
    {
        //Models.Add(new Model() { ModelId = "text-davinci-003" });
        //Models.Add(new Model() { ModelId = "text-davinci-004" });
        //Models.Add(new Model() { ModelId = "text-davinci-005" });
        //Models.Add(new Model() { ModelId = "text-davinci-006" });
    }

    public Model SelectedModel
    {
        get { return selectedModel; }
        set
        {
            if (selectedModel != value)
            {
                selectedModel = value;
                OnPropertyChanged(nameof(SelectedModel));
            }
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


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
