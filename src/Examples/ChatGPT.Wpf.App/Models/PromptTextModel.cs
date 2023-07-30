namespace ChatGPT.Wpf.App.Models;

public sealed class PromptTextModel : BaseModel
{
    private string? text = default!;

    public string? Text
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
}



public sealed class InstructionTextModel : BaseModel
{
    private string? text = default!;

    public string? Text
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
}

public sealed class HintTextModel : BaseModel
{
    private string? text = default!;

    public string? Text
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
}
