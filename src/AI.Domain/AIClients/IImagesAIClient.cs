using AI.Domain.Models;

namespace AI.Domain.AIClients;

public interface IImagesAIClient
{
    Task<Response<GeneratedImage>?> CreateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken);

    Task<Response<GeneratedImage>?> CreateImageEditsAsync(GenerateEditedImageRequest request, CancellationToken cancellationToken);

    Task<Response<GeneratedImage>?> CreateImageVariationsAsync(GenerateVariationsOfImageRequest request, CancellationToken cancellationToken);

}