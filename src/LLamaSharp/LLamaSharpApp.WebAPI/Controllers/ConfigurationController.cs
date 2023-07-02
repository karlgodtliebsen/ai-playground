using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Embeddings API
/// Llma Embeddings Controller <a href="https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/embeddings/" />
/// API to get the embeddings of a text in LLM, for example, to train other MLP models.
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
[Authorize]
public class ConfigurationController : ControllerBase
{
    private readonly IOptionsService domainService;
    private readonly IUserIdProvider userProvider;
    private readonly ILogger<ConfigurationController> logger;

    /// <summary>
    /// Constructor for ConfigurationController
    /// </summary>
    /// <param name="domainService"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public ConfigurationController(IOptionsService domainService, IUserIdProvider userProvider, ILogger<ConfigurationController> logger)
    {
        this.logger = logger;
        this.domainService = domainService;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Finds the Users LlmaModel Options
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/modelparams")]
    public async Task<LlmaModelOptions> GetUsersLlamaModelConfiguration(CancellationToken cancellationToken)
    {
        return await domainService.GetLlmaModelOptions(userProvider.UserId, cancellationToken);
    }


    /// <summary>
    /// Finds the Users InferenceModel Options
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/inference")]
    public async Task<InferenceOptions> GetUsersInferenceModelConfiguration(CancellationToken cancellationToken)
    {
        return await domainService.GetInferenceOptions(userProvider.UserId, cancellationToken);
    }


    /// <summary>
    /// Update the model options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/modelparams")]
    public async Task UpdateLlmaModelOptions([FromBody] LlmaModelOptions request, CancellationToken cancellationToken)
    {
        await domainService.PersistLlmaModelOptions(request, userProvider.UserId, cancellationToken);
    }

    /// <summary>
    /// Update the inference options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/inference")]
    public async Task UpdateInferenceOptions([FromBody] InferenceOptions request, CancellationToken cancellationToken)
    {
        await domainService.PersistInferenceOptions(request, userProvider.UserId, cancellationToken);
    }

}
