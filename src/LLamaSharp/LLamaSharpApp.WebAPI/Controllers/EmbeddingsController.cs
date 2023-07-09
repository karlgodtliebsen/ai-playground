using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;
/// <summary>
/// Embeddings API
/// <a href="https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/embeddings/"/>
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
[Authorize]
public class EmbeddingsController : ControllerBase
{
    private readonly IEmbeddingsService domainService;
    private readonly IUserIdProvider userProvider;
    private readonly ILogger<EmbeddingsController> logger;

    /// <summary>
    /// Constructor for EmbeddingsController
    /// </summary>
    /// <param name="service"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public EmbeddingsController(IEmbeddingsService service, IUserIdProvider userProvider, ILogger<EmbeddingsController> logger)
    {
        this.logger = logger;
        domainService = service;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Get the embeddings of a text in LLM, for example, to train other MLP models.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("embeddings")]
    public async Task<float[]> Embeddings([FromBody] EmbeddingsRequest request, CancellationToken cancellationToken)
    {
        var model = new EmbeddingsMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlamaModelOptions = request.LlamaModelOptions,
            UserId = userProvider.UserId
        };

        return await domainService.GetEmbeddings(model, cancellationToken);
    }
}
