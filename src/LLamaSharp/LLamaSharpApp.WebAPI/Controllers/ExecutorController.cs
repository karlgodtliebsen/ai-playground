using System.ComponentModel;
using System.Text;

using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Executor Controller
/// <a href=" https://scisharp.github.io/LLamaSharp/0.4/LLamaExecutors/parameters/"/>
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InstructModeExecute.md"/>
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InteractiveModeExecute.md"/>
/// The word "inference" refers to the process of drawing conclusions or making deductions based on evidence, reasoning, or information that is available.
/// It involves using existing knowledge or data to reach a logical or reasonable understanding or interpretation.
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[DisplayName("Llma Executor Controller <a href=\"https://scisharp.github.io/LLamaSharp/0.4/LLamaExecutors/parameters/\">")]
[Description("API to execute text-to-text tasks with Inference parameters.")]
[ApiController]
[Route("api/llama")]
public class ExecutorController : ControllerBase
{
    private readonly IExecutorService domainService;
    private readonly ILogger<ExecutorController> logger;

    public ExecutorController(IExecutorService service, ILogger<ExecutorController> logger)
    {
        this.logger = logger;
        domainService = service;
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
    /// <param name="cancellationToken"></param>
    /// <returns>Inferred text</returns>
    [HttpPost("executor")]
    public async Task<string> ExecutorAsync([FromBody] ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        var requestModel = new ExecutorInferMessage(request.Text);
        if (request.UsePersistedModelState.HasValue) requestModel.UsePersistedModelState = request.UsePersistedModelState.Value;
        if (request.UsePersistedExecutorState.HasValue) requestModel.UsePersistedExecutorState = request.UsePersistedExecutorState.Value;
        if (request.UseStatelessExecutor.HasValue) requestModel.UseStatelessExecutor = request.UseStatelessExecutor.Value;
        requestModel.InferenceType = request.InferenceType;
        var sb = new StringBuilder();
        var result = domainService.Executor(requestModel, CancellationToken.None);
        await foreach (var s in result.WithCancellation(cancellationToken))
        {
            sb.Append(s);
        }
        return sb.ToString();
    }
}
