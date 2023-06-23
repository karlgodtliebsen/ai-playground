using OpenAI.Client.Models;

namespace ChatGPTClient.Dialogs.Models;

public class ChatCompletionOptionsViewModel : OptionsViewModelBase
{

    /// <summary>
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    /// </summary>
    public string? User { get; init; } = default!;

    public ChatCompletionMessage[] Messages { get; init; }

    public Functions[]? Functions { get; init; } = default!;


    public object? FunctionCall { get; init; } = default!;

}
