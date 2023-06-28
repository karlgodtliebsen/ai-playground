using System.Text.Json.Serialization;

namespace LLamaSharpApp.WebAPI.Configuration;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public class LlmaOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlmaOptions";

    //TODO: verify by making an integration test of configuration

    public string? Model { get; set; } = default!;

    ////TODO: move these two parameters
    //public string PromptFile { get; set; } = default!;

    //public string[] AntiPrompt { get; set; } = default!;

    [JsonPropertyName("n_ctx")]
    public int? ContextSize { get; set; } = 512;

    [JsonPropertyName("gpuLayerCount")]
    public int? GpuLayerCount { get; set; } = 20;

    [JsonPropertyName("seed")]
    public int? Seed { get; set; } = 1337;

    [JsonPropertyName("useFp16Memory")]
    public bool? UseFp16Memory { get; set; } = true;

    [JsonPropertyName("useMemorymap")]
    public bool? UseMemorymap { get; set; } = true;

    [JsonPropertyName("useMemoryLock")]
    public bool? UseMemoryLock { get; set; } = false;

    [JsonPropertyName("perplexity")]
    public bool? Perplexity { get; set; } = false;

    [JsonPropertyName("loraAdapter")]
    public string LoraAdapter { get; set; } = "";

    [JsonPropertyName("loraBase")]
    public string LoraBase { get; set; } = "";

    [JsonPropertyName("threads")]
    public int? Threads { get; set; } = -1;

    [JsonPropertyName("batchSize")]
    public int? BatchSize { get; set; } = 512;

    [JsonPropertyName("convertEosToNewLine")]
    public bool? ConvertEosToNewLine { get; set; } = false;

    [JsonPropertyName("embeddingMode")]
    public bool? EmbeddingMode { get; set; } = false;
}
