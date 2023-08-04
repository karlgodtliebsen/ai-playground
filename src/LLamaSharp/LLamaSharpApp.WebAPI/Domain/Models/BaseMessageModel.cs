using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Domain.Models;

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
    public LlamaModelOptions? ModelOptions { get; set; } = default!;
}
