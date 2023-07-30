using ChatGPT.Wpf.App.Dialogs.Models;
using ChatGPT.Wpf.App.Models;

namespace ChatGPT.Wpf.App.TabPages.Completions;

public class CompletionViewModel : BaseViewModel
{

    public CompletionOptionsViewModel Options { get; } = new();

    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
}
