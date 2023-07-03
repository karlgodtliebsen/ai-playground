using OneOf;
using OpenAI.Client.OpenAI.Models.Images;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IImagesAIClient
{
    Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken);

    Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageEditsAsync(GenerateEditedImageRequest request, CancellationToken cancellationToken);

    Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageVariationsAsync(GenerateVariationsOfImageRequest request, CancellationToken cancellationToken);

}
