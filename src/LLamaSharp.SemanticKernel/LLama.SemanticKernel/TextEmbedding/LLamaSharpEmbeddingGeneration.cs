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
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        return data.Select(text => new ReadOnlyMemory<float>(embedder.GetEmbeddings(text))).ToList();
    }
}
