using System.Text.Json.Serialization;
using OpenAI.Client.OpenAI.Models.ChatCompletion;

namespace OpenAI.Client.OpenAI.Models.Requests;

/// <summary>
/// https://platform.openai.com/docs/api-reference/chat/create
/// </summary>
public class ChatCompletionRequest : BaseRequest
{
    public ChatCompletionRequest()
    {
        RequestUri = "chat/completions";
    }

    /// <summary>
    /// A list of messages comprising the conversation so far. 
    /// </summary>
    [JsonPropertyName("messages")]
    public ChatCompletionMessage[] Messages { get; init; }

    /// <summary>
    /// A list of functions the model may generate JSON inputs for
    /// </summary>
    [JsonPropertyName("functions")]
    public Functions[]? Functions { get; init; } = default!;

    /// <summary>
    /// Controls how the model responds to function calls. "none" means the model does not call a function, and responds to the end-user.
    /// "auto" means the model can pick between an end-user or calling a function.
    /// Specifying a particular function via {"name":\ "my_function"} forces the model to call that function.
    /// "none" is the default when no functions are present.
    /// "auto" is the default if functions are present.
    /// </summary>
    [JsonPropertyName("function_call")]
    public object? FunctionCall { get; init; } = default!;

}
