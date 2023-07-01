using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Models;

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
    /// The request specific LlmaModelOptions: Optional
    /// </summary>
    public LlmaModelOptions? LlmaModelOptions { get; set; } = default!;
}
