﻿using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class Hyperparams
{
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("batch_size")]
    public int? BatchSize { get; set; } = default!;
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("learning_rate_multiplier")]
    public double? LearningRateMultiplier { get; set; } = default!;
    /// <summary> Id for completion response. </summary>
    [JsonPropertyName("n_epochs")]
    public int? Epochs { get; set; } = default!;

    [JsonPropertyName("prompt_loss_weight")]
    public double? PromptLossWeight { get; set; } = default!;

}