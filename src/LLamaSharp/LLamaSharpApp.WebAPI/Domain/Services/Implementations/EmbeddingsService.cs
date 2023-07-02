using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Embeddings Service
/// </summary>
public class EmbeddingsService : IEmbeddingsService
{
    private readonly ILlmaModelFactory factory;
    private readonly IOptionsService optionsService;
    private readonly ILogger<EmbeddingsService> logger;

    /// <summary>
    /// Constructor for Embeddings Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public EmbeddingsService(ILlmaModelFactory factory, IOptionsService optionsService, ILogger<EmbeddingsService> logger)
    {
        this.factory = factory;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// Finds the embeddings of a text and returns them
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<float[]> GetEmbeddings(EmbeddingsMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlmaModelOptions(input.UserId, cancellationToken);
        var embedder = factory.CreateEmbedder(modelOptions);
        var embeddings = embedder.GetEmbeddings(input.Text);
        return embeddings;
    }
}
