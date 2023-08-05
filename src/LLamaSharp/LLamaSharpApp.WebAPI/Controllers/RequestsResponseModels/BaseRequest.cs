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
}
