using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;
using LLamaSharpApp.WebAPI.Controllers.Services;
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
    private readonly IUserIdProvider userProvider;
    private readonly RequestMessagesMapper mapper;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for EmbeddingsController
    /// </summary>
    /// <param name="userProvider"></param>
    /// <param name="mapper"></param>
    /// <param name="logger"></param>
    public EmbeddingsController(IUserIdProvider userProvider, RequestMessagesMapper mapper, ILogger logger)
    {
        this.logger = logger;
        this.userProvider = userProvider;
        this.mapper = mapper;
    }

    /// <summary>
    /// Get the embeddings of a text in LLM, for example, to train other MLP models.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="domainService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("embeddings")]
    public async Task<float[]> Embeddings([FromBody] EmbeddingsRequest request, [FromServices] IEmbeddingsService domainService, CancellationToken cancellationToken)
    {
        var model = mapper.Map(request, userProvider.UserId);
        return await domainService.GetEmbeddings(model, cancellationToken);
    }
}
