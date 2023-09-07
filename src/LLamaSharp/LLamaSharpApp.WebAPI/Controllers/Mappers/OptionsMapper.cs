using LLamaSharp.Domain.Configuration;

using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

using Riok.Mapperly.Abstractions;

namespace LLamaSharpApp.WebAPI.Controllers.Mappers;

/// <summary>
/// Using Source Generator Mapping: Riok.Mapperly Mapper for Options mapping
/// </summary>
[Mapper]
public partial class OptionsMapper
{
    /// <summary>
    /// Maps the LlamaModelOptions to the LlamaModelRequestResponse
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public partial LlamaModelRequestResponse Map(LLamaModelOptions options);


    /// <summary>
    /// Maps the LlamaModelOptions to the LlamaModelRequestResponse and sanitizes the sensitive data (path information)
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public LlamaModelRequestResponse MapProtectSensitiveData(LLamaModelOptions options)
    {
        var response = Map(options);
        response.ModelName = options.GetSanitizeSensitiveData();
        return response;
    }

    /// <summary>
    /// Maps the LlamaModelRequestResponse to the LlamaModelOptions and restores the sensitive data (path information)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="defaultOptions"></param>
    /// <returns></returns>
    public LLamaModelOptions MapRestoreSensitiveData(LlamaModelRequestResponse? request, LLamaModelOptions defaultOptions)
    {
        if (request is null)
        {
            return defaultOptions;
        }
        var options = Map(request);
        options.ModelPath = defaultOptions.ModelPath;
        options.RestoreSanitizedSensitiveData(request.ModelName);
        return options;
    }


    /// <summary>
    /// Maps the LlamaModelRequestResponse to the LlamaModelOptions
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public partial LLamaModelOptions Map(LlamaModelRequestResponse options);

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
