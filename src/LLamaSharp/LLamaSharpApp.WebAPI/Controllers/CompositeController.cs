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
[Route("api/llama/composite")]
[Authorize]
//[AllowAnonymous]
public class CompositeController : ControllerBase
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
    public CompositeController(ICompositeService domainService, IUserIdProvider userProvider, RequestMessagesMapper mapper, ILogger logger)
    {
        this.domainService = domainService;
        this.logger = logger;
        this.userProvider = userProvider;
        this.mapper = mapper;
    }

    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// Matches LLama.Examples.NewVersion.ChatSessionStripRoleName
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("interactiveInstructionExecute/noroles")]
    public async Task<IActionResult> ChatSessionWithInstructionsExecutorAndStrippedRoleName([FromBody] ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        var op = logger.BeginOperation("Running Interactive Instruction Chat for {userId}...", userProvider.UserId);
        var requestModel = mapper.Map(request, userProvider.UserId);
        var result = await domainService.ChatSessionWithInstructionsExecutorAndNoRoleNames(requestModel, cancellationToken);
        return result.Match<IActionResult>(
            r =>
            {
                op.Complete();
                return Ok(r);
            },
            BadRequest
            );
    }

    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// Matches LLama.Examples.NewVersion.ChatSessionWithRoleName
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionWithRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("interactiveInstructionExecute/roles")]
    public async Task<IActionResult> ChatSessionWithInstructionsExecutorAndRoleName([FromBody] ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        var op = logger.BeginOperation("Running Interactive Instruction Chat for {userId}...", userProvider.UserId);
        var requestModel = mapper.Map(request, userProvider.UserId);
        var result = await domainService.ChatSessionWithInstructionsExecutorAndRoleNames(requestModel, cancellationToken);
        return result.Match<IActionResult>(
            r =>
            {
                op.Complete();
                return Ok(r);
            },
            BadRequest
        );
    }
}
