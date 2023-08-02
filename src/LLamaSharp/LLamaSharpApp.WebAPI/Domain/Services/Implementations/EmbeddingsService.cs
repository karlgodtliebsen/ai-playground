using LLamaSharpApp.WebAPI.Domain.Models;

using SerilogTimings.Extensions;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Embeddings Service
/// </summary>
public class EmbeddingsService : IEmbeddingsService
{
    private readonly ILlamaModelFactory factory;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for Embeddings Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public EmbeddingsService(ILlamaModelFactory factory, IOptionsService optionsService, ILogger logger)
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
        using var op = logger.BeginOperation("Creating Embeddings");
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var embedder = factory.CreateEmbedder(modelOptions);
        var embeddings = embedder.GetEmbeddings(input.Text);
        op.Complete();
        return embeddings;
    }
}
