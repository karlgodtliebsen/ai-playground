using OpenAI.Client.Models.Images;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IImagesAIClient
{
    Task<Response<GeneratedImage>?> CreateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken);

    Task<Response<GeneratedImage>?> CreateImageEditsAsync(GenerateEditedImageRequest request, CancellationToken cancellationToken);

    Task<Response<GeneratedImage>?> CreateImageVariationsAsync(GenerateVariationsOfImageRequest request, CancellationToken cancellationToken);

}