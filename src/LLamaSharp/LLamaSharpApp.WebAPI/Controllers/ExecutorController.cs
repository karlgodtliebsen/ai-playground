using System.ComponentModel;

using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Models.Requests;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// https://scisharp.github.io/LLamaSharp/0.4/LLamaExecutors/parameters/
/// The word "inference" refers to the process of drawing conclusions or making deductions based on evidence, reasoning, or information that is available.
/// It involves using existing knowledge or data to reach a logical or reasonable understanding or interpretation.
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[DisplayName("Llma Executor Controller <a href=\"https://scisharp.github.io/LLamaSharp/0.4/LLamaExecutors/parameters/\">")]
[Description("API to execute text-to-text tasks with Inference parameters.")]
//[Authorize] Introduced later on
[ApiController]
//[Route("[controller]")]
[Route("api/llama")]
public class ExecutorController : ControllerBase
{
    private readonly IExecutorService domainService;
    private readonly ILogger<ExecutorController> logger;

    public ExecutorController(ILogger<ExecutorController> logger,
        IExecutorService service)
    {
        this.logger = logger;
        domainService = service;
    }


    [HttpPost("executor")]
    public IAsyncEnumerable<string> ExecutorAsync([FromBody] ExecutorInferRequest request, CancellationToken cancellationToken)
    {
        var requestModel = new ExecutorInferMessage(request.Text);
        if (request.UsePersistedModelState.HasValue) requestModel.UsePersistedModelState = request.UsePersistedModelState.Value;
        if (request.UsePersistedExecutorState.HasValue) requestModel.UsePersistedExecutorState = request.UsePersistedExecutorState.Value;
        if (request.UseStatelessExecutor.HasValue) requestModel.UseStatelessExecutor = request.UseStatelessExecutor.Value;
        return domainService.Executor(requestModel, CancellationToken.None);
    }
}
