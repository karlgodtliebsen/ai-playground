﻿using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class CompletionRequest : ExtendedModelRequest
{
    /// <summary>
    /// Constructor for Completions Request
    /// </summary>
    public CompletionRequest()
    {
        RequestUri = "completions";
    }


    [JsonIgnore]
    public string[]? UsePrompts
    {
        set
        {
            if (value is not null)
            {
                Prompt = string.Join("\n", value);
            }
            else
            {
                Prompt = default!;
            }
        }
    }


    [JsonPropertyName("prompt")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Prompt { get; set; } = default!;

    /// <summary>
    /// The suffix that comes after a completion of inserted text.  Defaults to null.
    /// <a href="https://platform.openai.com/docs/guides/gpt/inserting-text" />
    /// </summary>
    [JsonPropertyName("suffix")]
    public string? Suffix { get; init; } = default!;


    /// <summary>
    /// Echo back the prompt in addition to the completion.  Defaults to false.
    /// </summary>
    [JsonPropertyName("echo")]
    public bool? Echo { get; init; } = default!;


    /// <summary>
    /// Generates best_of completions server-side and returns the "best" (the one with the highest log probability per token). Results cannot be streamed.
    /// When used with n, best_of controls the number of candidate completions and n specifies how many to return – best_of must be greater than n.
    /// Note: Because this parameter generates many completions, it can quickly consume your token quota.Use carefully and ensure that you have reasonable settings for max_tokens and stop.
    /// </summary>
    [JsonPropertyName("best_of")]
    public int? BestOf { get; init; } = default!;

}
