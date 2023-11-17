using LLama;

using Microsoft.SemanticKernel.AI.Embeddings;

namespace LLamaSharp.SemanticKernel.TextEmbedding;

public sealed class LLamaSharpEmbeddingGeneration : ITextEmbeddingGeneration
{
    private readonly LLamaEmbedder embedder;

    public LLamaSharpEmbeddingGeneration(LLamaEmbedder embedder)
    {
        this.embedder = embedder;
    }

    /// <inheritdoc/>
    public Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        var result = data.Select(text => new ReadOnlyMemory<float>(embedder.GetEmbeddings(text))).ToList();
        return Task.FromResult<IList<ReadOnlyMemory<float>>>(result);
    }
}
