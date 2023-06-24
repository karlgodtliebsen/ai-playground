using OneOf;

using OpenAI.Client.Models.Images;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IImagesAIClient
{
    Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken);

    Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageEditsAsync(GenerateEditedImageRequest request, CancellationToken cancellationToken);

    Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageVariationsAsync(GenerateVariationsOfImageRequest request, CancellationToken cancellationToken);

}
