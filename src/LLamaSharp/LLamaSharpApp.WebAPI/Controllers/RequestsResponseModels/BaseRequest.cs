namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

/// <summary>
/// Base type for requests
/// </summary>
public class BaseRequest
{
    /// <summary>
    /// Request specific LlamaModelOptions
    /// </summary>
    public LlamaModelRequestResponse? ModelOptions { get; set; } = default!;

    /// <summary>
    /// AntiPrompt
    /// </summary>
    public string[]? AntiPrompts { get; set; } = default!;

    /// <summary>
    ///Prompt
    /// </summary>
    public string? Prompt { get; set; } = default!;
}
