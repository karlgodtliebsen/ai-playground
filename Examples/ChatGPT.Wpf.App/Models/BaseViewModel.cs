namespace ChatGPT.Wpf.App.Models;

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
