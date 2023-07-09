using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// Base type for requests
/// </summary>
public class BaseRequest
{
    /// <summary>
    /// Request specific LlamaModelOptions
    /// </summary>
    public LlamaModelOptions? LlamaModelOptions { get; set; } = default!;
}
