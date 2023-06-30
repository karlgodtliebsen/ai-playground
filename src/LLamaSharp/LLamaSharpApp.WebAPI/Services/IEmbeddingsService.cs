using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

public interface IEmbeddingsService
{
    float[] GetEmbeddings(EmbeddingsMessage input);
}

