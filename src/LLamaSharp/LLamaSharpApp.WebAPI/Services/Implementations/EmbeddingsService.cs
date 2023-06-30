using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services.Implementations;

/// <summary>
/// Embeddings Service
/// </summary>
public class EmbeddingsService : IEmbeddingsService
{
    private readonly ILlmaModelFactory factory;
    private readonly ILogger<EmbeddingsService> logger;

    public EmbeddingsService(ILlmaModelFactory factory, ILogger<EmbeddingsService> logger)
    {
        this.factory = factory;
        this.logger = logger;
    }

    /// <summary>
    /// Finds the embeddings of a text and returns them
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public float[] GetEmbeddings(EmbeddingsMessage input)
    {
        var embedder = factory.CreateEmbedder();
        var embeddings = embedder.GetEmbeddings(input.Text);
        return embeddings;
    }
}
