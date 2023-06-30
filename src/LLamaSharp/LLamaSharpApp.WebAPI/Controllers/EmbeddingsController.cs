using System.ComponentModel;

using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Models.Requests;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;
/// <summary>
/// Embeddings API
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[DisplayName("Llma Embeddings Controller <a href=\"https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/embeddings/\">")]
[Description("API to get the embeddings of a text in LLM, for example, to train other MLP models..")]
[ApiController]
[Route("api/llama")]
public class EmbeddingsController : ControllerBase
{
    private readonly IEmbeddingsService domainService;
    private readonly ILogger<EmbeddingsController> logger;

    public EmbeddingsController(ILogger<EmbeddingsController> logger,
        IEmbeddingsService service)
    {
        this.logger = logger;
        domainService = service;
    }

    [HttpPost("embeddings")]
    public float[] Embeddings([FromBody] EmbeddingsRequest request)
    {
        var requestModel = new EmbeddingsMessage(request.Text);
        return domainService.GetEmbeddings(requestModel);
    }
}
