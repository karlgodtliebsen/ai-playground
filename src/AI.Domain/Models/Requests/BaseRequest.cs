using System.Text.Json.Serialization;

namespace AI.Domain.Models.Requests;

public class BaseRequest
{
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; init; } = 16;

    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; } = 1.0;

    [JsonPropertyName("top_p")]
    public double? TopP { get; init; } = 1.0;

    [JsonPropertyName("n")]
    public int? NumChoicesPerPrompt { get; init; } = default!;

    [JsonPropertyName("stream")]
    public bool? Stream { get; init; } = default!;


    /// <summary>
    /// Include the log probabilities on the logprobs most likely tokens, which can be found in <see cref="CompletionResult.Completions"/> -> <see cref="Choice.Logprobs"/>. So for example, if logprobs is 5, the API will return a list of the 5 most likely tokens. If logprobs is supplied, the API will always return the logprob of the sampled token, so there may be up to logprobs+1 elements in the response.  The maximum value for logprobs is 5.
    /// </summary>
    [JsonPropertyName("logprobs")]
    public int? Logprobs { get; init; } = default!;

    /// <summary>
    /// Echo back the prompt in addition to the completion.  Defaults to false.
    /// </summary>
    [JsonPropertyName("stop")]
    public string? Stop { get; init; } = default!;


    /// <summary>
    /// The scale of the penalty applied if a token is already present at all.  Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.  Defaults to 0.
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double? PresencePenalty { get; init; } = default!;

    /// <summary>
    /// The scale of the penalty for how often a token is used.  Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.  Defaults to 0.
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double? FrequencyPenalty { get; init; } = default!;

    /// <summary>
    /// Modify the likelihood of specified tokens appearing in the completion.
    /// </summary>
    [JsonPropertyName("logit_bias")]
    public Dictionary<string, string>? LogitBias { get; init; } = default!;


    /// <summary>
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; init; } = default!;

}