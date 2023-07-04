using ChatGPTClient.Dialogs.Models;

using OpenAI.Client.OpenAI.Models.ChatCompletion;

namespace ChatGPT.Wpf.App.Dialogs.Models;

public class ChatCompletionOptionsViewModel : OptionsViewModelBase
{

    /// <summary>
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    /// </summary>
    public string? User { get; init; } = default!;
    /// <summary>
    /// 
    /// </summary>
    public ChatCompletionMessage[] Messages { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public Functions[]? Functions { get; init; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public object? FunctionCall { get; init; } = default!;

}
