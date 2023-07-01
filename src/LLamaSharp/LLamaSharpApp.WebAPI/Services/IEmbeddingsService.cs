using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

/// <summary>
/// Handles Embeddings
/// </summary>
public interface IEmbeddingsService
{
    /// <summary>
    /// get the embeddings for the input text
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<float[]> GetEmbeddings(EmbeddingsMessage input, CancellationToken cancellationToken);
}

