﻿using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class EmbeddingsData
{

    /// <summary> Object for completion response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "embedding";

    /// <summary> Created time for completion response. </summary>
    [JsonPropertyName("embedding")]
    public double[] Embedding { get; set; }

    [JsonPropertyName("index")]
    public long Index { get; set; }

}