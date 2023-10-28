namespace LLamaSharp.WebAPI.Client.Clients.Models;

/// <summary>
/// Base type for requests
/// </summary>
public class BaseModel
{
    /// <summary>
    /// Request specific LlamaModelOptions
    /// </summary>
    public LlamaModel? ModelOptions { get; set; } = default!;

    /// <summary>
    /// Request specific InferenceOptions
    /// </summary>
    public InferenceModel? InferenceOptions { get; set; } = default!;


    /// <summary>
    /// AntiPrompt
    /// </summary>
    public string[]? AntiPrompts { get; set; } = default!;

    /// <summary>
    ///Prompt
    /// </summary>
    public string? Prompt { get; set; } = default!;
}
