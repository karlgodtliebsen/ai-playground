using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
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
    public float[] GetEmbeddings([FromBody] GetEmbeddingsRequest request)
    {
        var requestModel = new GetEmbeddings(request.Text);
        return domainService.GetEmbeddings(requestModel);
    }
}
