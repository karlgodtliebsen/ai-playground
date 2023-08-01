using System.Text;

using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    private readonly ILogger<ExecutorController> logger;

    /// <summary>
    /// Constructor for ExecutorController
    /// </summary>
    /// <param name="service"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public ExecutorController(IUserIdProvider userProvider, ILogger<ExecutorController> logger)
    {
        this.logger = logger;
        this.userProvider = userProvider;
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
        var requestModel = new ExecutorInferMessage(request.Text)
        {
            InferenceType = request.InferenceType,
            LlamaModelOptions = request.LlamaModelOptions,
            InferenceOptions = request.InferenceOptions,
            UserId = userProvider.UserId
        };
        if (request.UsePersistedModelState.HasValue) requestModel.UsePersistedModelState = request.UsePersistedModelState.Value;
        if (request.UsePersistedExecutorState.HasValue) requestModel.UsePersistedExecutorState = request.UsePersistedExecutorState.Value;
        if (request.UseStatelessExecutor.HasValue) requestModel.UseStatelessExecutor = request.UseStatelessExecutor.Value;

        var sb = new StringBuilder();
        var result = domainService.Executor(requestModel, CancellationToken.None);
        await foreach (var s in result.WithCancellation(cancellationToken))
        {
            sb.Append(s);
        }
        return sb.ToString();
    }
}
