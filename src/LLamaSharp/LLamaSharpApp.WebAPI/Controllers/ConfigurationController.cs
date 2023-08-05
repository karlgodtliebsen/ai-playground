using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Embeddings API
/// Llma Configuration Controller <a href="https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/embeddings/" />
/// API to get the Configuration for LLama Model and Inference
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
    /// <param name="mapper"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/modelparams")]
    public async Task<LlamaModelRequestResponse> GetUsersLlamaModelConfiguration([FromServices] Mappers.OptionsMapper mapper, CancellationToken cancellationToken)
    {
        var options = await domainService.GetLlamaModelOptions(userProvider.UserId, cancellationToken);
        var response = mapper.MapProtectSensitiveData(options);
        return response;
    }


    /// <summary>
    /// Finds the Users InferenceModel Options
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/inference")]
    public async Task<InferenceRequestResponse> GetUsersInferenceModelConfiguration([FromServices] Mappers.OptionsMapper mapper, CancellationToken cancellationToken)
    {
        var options = await domainService.GetInferenceOptions(userProvider.UserId, cancellationToken);
        var response = mapper.Map(options);
        return response;
    }


    /// <summary>
    /// Returns the Systems Prompt/chat Templates
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration/prompt-templates")]
    public async Task GetPromptTemplates(CancellationToken cancellationToken)
    {
        const string sep = "\n------------------------------------------------------------------------------------------------------\n";//should be utf8 literal, but...
        Response.ContentType = "text/plain";
        await foreach (var template in domainService.GetSystemPromptTemplates(cancellationToken))
        {
            await Response.WriteAsync(sep, cancellationToken);
            await Response.WriteAsync(template, cancellationToken);
            await Response.WriteAsync("\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        await Response.CompleteAsync();
    }


    /// <summary>
    /// Update the model options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="mapper"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/modelparams")]
    public async Task UpdateLlamaModelOptions([FromBody] LlamaModelRequestResponse request, [FromServices] Mappers.OptionsMapper mapper, CancellationToken cancellationToken)
    {
        LlamaModelOptions defaultOptions = domainService.GetDefaultLlamaModelOptions();
        var options = mapper.MapRestoreSensitiveData(request, defaultOptions);
        await domainService.PersistLlamaModelOptions(options, userProvider.UserId, cancellationToken);
    }

    /// <summary>
    /// Update the inference options for the user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="mapper"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("configuration/inference")]
    public async Task UpdateInferenceOptions([FromBody] InferenceRequestResponse request, [FromServices] Mappers.OptionsMapper mapper, CancellationToken cancellationToken)
    {
        var options = mapper.Map(request);
        await domainService.PersistInferenceOptions(options, userProvider.UserId, cancellationToken);
    }
}
