using LLamaSharp.Domain.Domain.DomainServices;

using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;
using LLamaSharpApp.WebAPI.Controllers.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SerilogTimings.Extensions;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Controller for Composite instructed Chat sessions using prompt templates
/// <a href="https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/parameters/">LLamaModel Parameters</a>
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
[Authorize]
//[AllowAnonymous]
public class GeneralController : ControllerBase
{
    private readonly IUserIdProvider userProvider;
    private readonly RequestMessagesMapper mapper;
    private readonly ICompositeService domainService;
    private readonly ILogger logger;

    /// <summary>
    /// Controller for Chat
    /// </summary>
    /// <param name="domainService"></param>
    /// <param name="userProvider"></param>
    /// <param name="mapper"></param>
    /// <param name="logger"></param>
    public GeneralController(ICompositeService domainService, IUserIdProvider userProvider, RequestMessagesMapper mapper, ILogger logger)
    {
        this.domainService = domainService;
        this.logger = logger;
        this.userProvider = userProvider;
        this.mapper = mapper;
    }

    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("execute")]
    public async Task<string> Chat([FromBody] ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("Running Interactive Instruction Chat for {userId}...", userProvider.UserId);
        var requestModel = mapper.Map(request, userProvider.UserId);
        var result = await domainService.ChatUsingInstructionsSessionWithRoleName(requestModel, cancellationToken);
        op.Complete();
        return result;
    }

}
