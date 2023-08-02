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
//[AllowAnonymous]
public class ConfigurationController : ControllerBase
{
    private readonly IOptionsService domainService;
    private readonly IUserIdProvider userProvider;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for ConfigurationController
    /// </summary>
    /// <param name="domainService"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public ConfigurationController(IOptionsService domainService, IUserIdProvider userProvider, ILogger logger)
    {
        this.logger = logger;
        this.domainService = domainService;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Finds the Users LlamaModel Options
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/modelparams")]
    public async Task<LlamaModelOptions> GetUsersLlamaModelConfiguration(CancellationToken cancellationToken)
    {
        return await domainService.GetLlamaModelOptions(userProvider.UserId, cancellationToken);
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
    /// Returns the Systems Prompt/chat Templates
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/chat-templates")]
    public async Task GetChatTemplates(CancellationToken cancellationToken)
    {
        Response.ContentType = "text/plain";
        await foreach (var template in domainService.GetSystemChatTemplates(cancellationToken))
        {
            await Response.WriteAsync(template, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        await Response.CompleteAsync();
    }


    /// <summary>
    /// Update the model options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/modelparams")]
    public async Task UpdateLlamaModelOptions([FromBody] LlamaModelOptions request, CancellationToken cancellationToken)
    {
        await domainService.PersistLlamaModelOptions(request, userProvider.UserId, cancellationToken);
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
