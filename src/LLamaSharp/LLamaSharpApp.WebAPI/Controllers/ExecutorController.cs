using System.Text;
using LLamaSharp.Domain.Domain.Services;
using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;
using LLamaSharpApp.WebAPI.Controllers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SerilogTimings.Extensions;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Executor Controller handles Inference
/// <a href=" https://scisharp.github.io/LLamaSharp/0.4/LLamaExecutors/parameters/"/>
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InstructModeExecute.md"/>
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InteractiveModeExecute.md"/>
/// The word "inference" refers to the process of drawing conclusions or making deductions based on evidence, reasoning, or information that is available.
/// It involves using existing knowledge or data to reach a logical or reasonable understanding or interpretation.
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
[Authorize]
public class ExecutorController : ControllerBase
{
    private readonly IUserIdProvider userProvider;
    private readonly RequestMessagesMapper mapper;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for ExecutorController
    /// </summary>
    /// <param name="userProvider"></param>
    /// <param name="mapper"></param>
    /// <param name="logger"></param>
    public ExecutorController(IUserIdProvider userProvider, RequestMessagesMapper mapper, ILogger logger)
    {
        this.logger = logger;
        this.userProvider = userProvider;
        this.mapper = mapper;
    }

    /// <summary>
    /// Uses Inference
    /// The word "inference" refers to the process of drawing conclusions or making deductions based on evidence, reasoning, or information that is available.
    /// It involves using existing knowledge or data to reach a logical or reasonable understanding or interpretation.
    /// </summary>
    /// <param name="request">
    /// Discriminator for the stateful Executor type
    /// Will be ignored when UseStatelessExecutor is true
    /// May be one of the following:InteractiveExecutor or InstructExecutor 
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InstructModeExecute.md"/>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InteractiveModeExecute.md"/>
    /// </param>
    /// <param name="domainService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Inferred text</returns>
    [HttpPost("executor")]
    public async Task<string> ExecutorAsync([FromBody] ExecutorInferRequest request, [FromServices] IExecutorService domainService, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("Running Executor for {userId}...", userProvider.UserId);
        var requestModel = mapper.Map(request, userProvider.UserId);
        var sb = new StringBuilder();
        var result = domainService.Executor(requestModel, cancellationToken);
        await foreach (var s in result.WithCancellation(cancellationToken))
        {
            sb.Append(s);
        }
        op.Complete();
        return sb.ToString();
    }
}
