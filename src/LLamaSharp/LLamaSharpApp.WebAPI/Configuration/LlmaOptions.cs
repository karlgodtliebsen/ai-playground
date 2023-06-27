using System.Text.Json.Serialization;

namespace LLamaSharpApp.WebAPI.Configuration;

public class LlmaOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public const string ConfigSectionName = "LlmaOptions";

    //TODO: remove the defaults
    //TODO: verify by making an integration test of configuration

    public string Model { get; set; } = "ggml-model-q4_0.bin";


    public string PromptFile { get; set; } = @"Assets\chat-with-bob.txt";

    public string[] AntiPrompt { get; set; } = new string[] { "User:" };

    [JsonPropertyName("n_ctx")]
    public int MaximumNumberOfTokens { get; set; } = 512;

    [JsonPropertyName("interactive")]
    public bool Interactive { get; set; } = true;

    [JsonPropertyName("repeat_penalty")]
    public float RepeatPenalty { get; set; } = 1.0f;

    [JsonPropertyName("verbose_prompt")]
    public bool VerbosePrompt { get; set; } = false;
}
