using LLamaSharpApp.WebAPI.Configuration;

namespace LLamaSharpApp.WebAPI.Controllers.Requests;

/// <summary>
/// Base type for requests
/// </summary>
public class BaseRequest
{
    /// <summary>
    /// Request specific LlmaModelOptions
    /// </summary>
    public LlmaModelOptions? LlmaModelOptions { get; set; } = default!;
}
