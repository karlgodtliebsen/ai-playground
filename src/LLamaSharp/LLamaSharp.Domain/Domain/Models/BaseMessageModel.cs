using LLamaSharp.Domain.Configuration;

namespace LLamaSharp.Domain.Domain.Models;

/// <summary>
/// Domain Model for SimpleTextMessage
/// </summary>
public class BaseMessageModel
{
    /// <summary>
    /// Constructor
    /// </summary>
    public BaseMessageModel()
    {
    }

    /// <summary>
    /// The request specific LlamaModelOptions: Optional
    /// </summary>
    public LLamaModelOptions? ModelOptions { get; set; } = default!;

    /// <summary>
    /// AntiPrompt
    /// </summary>
    public string[]? AntiPrompts { get; set; } = default!;


    /// <summary>
    ///Prompt
    /// </summary>
    public string? Prompt { get; set; } = default!;
}
