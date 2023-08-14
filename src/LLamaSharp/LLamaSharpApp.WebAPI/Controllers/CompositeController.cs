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
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("executeInstruction")]
    public async Task<IActionResult> ChatSessionWithInstructionsExecutorAndRoleName([FromBody] ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        var op = logger.BeginOperation("Running Interactive Instruction Chat for {userId}...", userProvider.UserId);
        var requestModel = mapper.Map(request, userProvider.UserId);
        var result = await domainService.ChatSessionWithInstructionsExecutorAndRoleName(requestModel, cancellationToken);
        //<OneOf<string,ErrorResponse>>

        return result.Match<IActionResult>(
            r =>
            {
                op.Complete();
                op.Dispose();
                return Ok(r);
            },
            BadRequest
            );
    }

}
