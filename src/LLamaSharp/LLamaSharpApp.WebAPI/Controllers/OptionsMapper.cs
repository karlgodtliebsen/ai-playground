using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

using Riok.Mapperly.Abstractions;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
///  Riok.Mapperly Mapper for Options
/// </summary>
[Mapper]
public partial class OptionsMapper
{
    /// <summary>
    /// Maps the LlamaModelOptions to the LlamaModelRequestResponse
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public partial LlamaModelRequestResponse Map(LlamaModelOptions options);

    /// <summary>
    /// Maps the LlamaModelRequestResponse to the LlamaModelOptions
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public partial LlamaModelOptions Map(LlamaModelRequestResponse options);

    /// <summary>
    /// Maps the InferenceOptions to the InferenceRequestResponse
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public partial InferenceRequestResponse Map(InferenceOptions options);

    /// <summary>
    /// Maps the InferenceRequestResponse to the InferenceOptions
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public partial InferenceOptions Map(InferenceRequestResponse options);


}
