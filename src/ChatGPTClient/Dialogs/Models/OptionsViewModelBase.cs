using System.Collections.Generic;
using System.ComponentModel;

namespace ChatGPTClient.Dialogs.Models;

public class OptionsViewModelBase : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public int MaxTokens { get; init; } = 100;

    public float Temperature { get; init; } = 0.1000f;

    public float TopP { get; init; } = 1.000f;

    public int NumChoicesPerPrompt { get; init; } = 1;

    public bool Stream { get; init; } = false;


    /// <summary>
    /// Include the log probabilities on the logprobs most likely tokens, which can be found in <see cref="CompletionResult.Completions"/> -> <see cref="Choice.Logprobs"/>. So for example, if logprobs is 5, the API will return a list of the 5 most likely tokens. If logprobs is supplied, the API will always return the logprob of the sampled token, so there may be up to logprobs+1 elements in the response.  The maximum value for logprobs is 5.
    /// </summary>
    public int? Logprobs { get; init; } = default!;

    /// <summary>
    /// Echo back the prompt in addition to the completion.  Defaults to false.
    /// Up to 4 sequences where the API will stop generating further tokens.
    /// </summary>
    public string[]? Stop { get; init; } = default!;


    /// <summary>
    /// The scale of the penalty applied if a token is already present at all.  Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.  Defaults to 0.
    /// </summary>
    public float PresencePenalty { get; init; } = default!;

    /// <summary>
    /// The scale of the penalty for how often a token is used.  Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.  Defaults to 0.
    /// </summary>
    public float FrequencyPenalty { get; init; } = default!;

    /// <summary>
    /// Modify the likelihood of specified tokens appearing in the completion.
    /// </summary>
    public Dictionary<string, string>? LogitBias { get; init; } = default!;


}
