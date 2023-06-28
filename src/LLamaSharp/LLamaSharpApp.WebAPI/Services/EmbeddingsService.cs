using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

public class EmbeddingsService : IEmbeddingsService
{
    private readonly ILlmaModelFactory factory;

    public EmbeddingsService(ILlmaModelFactory factory)
    {
        this.factory = factory;
    }

    public float[] GetEmbeddings(GetEmbeddings input)
    {
        var embedder = factory.CreateEmbedder();
        float[] embeddings = embedder.GetEmbeddings(input.Text);
        return embeddings;
    }
}
