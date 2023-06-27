using ChatGPTClient.Dialogs.Models;
using ChatGPTClient.Models;

namespace ChatGPTClient;

public class CompletionViewModel : BaseViewModel
{

    public CompletionOptionsViewModel Options { get; } = new();

    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
}
