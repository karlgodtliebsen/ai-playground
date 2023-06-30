using System.ComponentModel;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Embeddings API
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[DisplayName("Llma Embeddings Controller <a href=\"https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/embeddings/\">")]
[Description("API to get the embeddings of a text in LLM, for example, to train other MLP models..")]
[ApiController]
[Route("api/llama")]
public class ConfigurationController : ControllerBase
{
    private readonly IOptionsService domainService;
    private readonly ILogger<ConfigurationController> logger;

    public ConfigurationController(IOptionsService domainService, ILogger<ConfigurationController> logger)
    {
        this.logger = logger;
        this.domainService = domainService;
    }

    /// <summary>
    /// Finds the Users LlmaModel Options
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/modelparams")]
    public async Task<LlmaModelOptions> GetUsersLlamaModelConfiguration(CancellationToken cancellationToken)
    {
        return await domainService.GetLlmaModelOptions("42", cancellationToken);
    }


    /// <summary>
    /// Finds the Users InferenceModel Options
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/inference")]
    public async Task<InferenceOptions> GetUsersInferenceModelConfiguration(CancellationToken cancellationToken)
    {
        return await domainService.GetInferenceOptions("42", cancellationToken);
    }


    /// <summary>
    /// Update the model options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/modelparams")]
    public async Task UpdateLlmaModelOptions([FromBody] LlmaModelOptions request, CancellationToken cancellationToken)
    {
        await domainService.PersistLlmaModelOptions(request, "42", cancellationToken);
    }

    /// <summary>
    /// Update the inference options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/inference")]
    public async Task UpdateInferenceOptions([FromBody] InferenceOptions request, CancellationToken cancellationToken)
    {
        await domainService.PersistInferenceOptions(request, "42", cancellationToken);
    }

}
