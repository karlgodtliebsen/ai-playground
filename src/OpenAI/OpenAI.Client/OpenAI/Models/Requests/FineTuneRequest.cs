using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class FineTuneRequest : ModelBaseRequest
{
    public FineTuneRequest()
    {
        RequestUri = "fine-tunes";
    }

    [JsonPropertyName("training_file")]
    public string TrainingFile { get; set; }


    [JsonPropertyName("validation_file")]
    public string? ValidationFile { get; set; } = default!;


    [JsonPropertyName("n_epochs")]
    public int? Epochs { get; set; } = default!;


    [JsonPropertyName("batch_size")]
    public int? BatchSize { get; set; } = default!;


    [JsonPropertyName("learning_rate")]
    public double? LearningRate { get; set; } = default!;


    [JsonPropertyName("prompt_loss_weight")]
    public double? PromptLossWeight { get; set; } = default!;


    [JsonPropertyName("compute_classification_metrics")]
    public bool? ComputeClassificationMetrics { get; set; } = default!;


    [JsonPropertyName("classification_n_classes")]
    public int? ClassificationNClasses { get; set; } = default!;


    [JsonPropertyName("classification_positive_class")]
    public string? ClassificationPositiveClass { get; set; } = default!;


    [JsonPropertyName("classification_betas")]

    public double[]? ClassificationBetas { get; set; } = default!;


    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; } = default!;

}
