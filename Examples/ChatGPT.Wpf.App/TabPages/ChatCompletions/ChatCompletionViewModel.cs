using ChatGPT.Wpf.App.Dialogs.Models;
using ChatGPT.Wpf.App.Models;

namespace ChatGPT.Wpf.App.TabPages.ChatCompletions;

public class ChatCompletionViewModel : BaseViewModel
{
    public CompletionOptionsViewModel Options { get; } = new();
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();

}
