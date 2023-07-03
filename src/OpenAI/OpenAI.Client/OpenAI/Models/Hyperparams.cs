using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models;

public class HyperParams
{
    /// <summary>
    /// Id for completion response.
    /// </summary>
    [JsonPropertyName("batch_size")]
    public int? BatchSize { get; set; } = default!;
    /// <summary>
    /// Id for completion response.
    /// </summary>
    [JsonPropertyName("learning_rate_multiplier")]
    public double? LearningRateMultiplier { get; set; } = default!;
    /// <summary>
    /// Id for completion response.
    /// </summary>
    [JsonPropertyName("n_epochs")]
    public int? Epochs { get; set; } = default!;

    [JsonPropertyName("prompt_loss_weight")]
    public double? PromptLossWeight { get; set; } = default!;

}
