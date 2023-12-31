﻿using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class ExtendedEmbeddingsRequest : ModelBaseRequest
{
    /// <summary>
    /// <a href="https://platform.openai.com/docs/guides/embeddings/what-are-embeddings" />
    /// </summary>
    public ExtendedEmbeddingsRequest()
    {
        RequestUri = "embeddings";
        //text-embedding-ada-002 
    }

    [JsonPropertyName("input")]
    ///Input text to embed, encoded as a string or array of tokens.
    /// To embed multiple inputs in a single request, pass an array of strings or array of token arrays.
    /// Each input must not exceed the max input tokens for the model (8191 tokens for text-embedding-ada-002).
    public string[] Input { get; init; }


    /// <summary>
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; init; } = default!;
}
