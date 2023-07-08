using System.Net.Http.Json;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.Chat;
using OpenAI.Client.OpenAI.Models.Images;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

using SerilogTimings.Extensions;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class ImagesAIClient : AIClientBase, IImagesAIClient
{
    //https://platform.openai.com/docs/api-reference/images/create-edit

    public ImagesAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    private async Task<OneOf<GeneratedImage, ErrorResponse>> UploadImage(string subUri, GenerateVariationsOfImageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var op = logger.BeginOperation($"UploadImageAsync for Variations {subUri}");
            using var content = new MultipartFormDataContent();
            using var imageData = new MemoryStream();
            await request.ImageStream.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(imageData.ToArray()), "image", request.Image);
            content.Add(new StringContent(request.NumberOfImagesToGenerate.ToString()), "n");
            content.Add(new StringContent(request.Size), "size");
            content.Add(new StringContent(request.ResponseFormat), "response_format");

            if (!string.IsNullOrWhiteSpace(request.User))
            {
                content.Add(new StringContent(request.User), ChatMessageRole.User.ToString());
            }
            PrepareClient();
            var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneratedImage>(cancellationToken: cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "PostAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }


    private async Task<OneOf<GeneratedImage, ErrorResponse>> UploadImage(string subUri, GenerateEditedImageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var op = logger.BeginOperation($"UploadImageAsync For Edit {subUri}");
            using var content = new MultipartFormDataContent();
            using var imageData = new MemoryStream();
            await request.ImageStream.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
            content.Add(new ByteArrayContent(imageData.ToArray()), "image", request.Image);
            content.Add(new StringContent(request.NumberOfImagesToGenerate.ToString()), "n");
            content.Add(new StringContent(request.Size), "size");
            content.Add(new StringContent(request.ResponseFormat), "response_format");
            content.Add(new StringContent(request.Prompt), "prompt");
            if (!string.IsNullOrEmpty(request.Mask) && request.MaskStream is not null)
            {
                using var maskData = new MemoryStream();
                await request.MaskStream.CopyToAsync(maskData, cancellationToken).ConfigureAwait(false);
                content.Add(new ByteArrayContent(maskData.ToArray()), "mask", request.Mask);
            }
            if (!string.IsNullOrWhiteSpace(request.User))
            {
                content.Add(new StringContent(request.User), ChatMessageRole.User.ToString());
            }
            PrepareClient();
            var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GeneratedImage>(cancellationToken: cancellationToken);
            op.Complete();
            return result!;

        }
        catch (Exception ex)
        {
            logger.Error(ex, "PostAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    public async Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<ImageGenerationRequest, GeneratedImage>(request.RequestUri, request, cancellationToken);
        return result;
    }

    public async Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageEditsAsync(GenerateEditedImageRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadImage(request.RequestUri, request, cancellationToken);
        return result;
    }
    public async Task<OneOf<GeneratedImage, ErrorResponse>> CreateImageVariationsAsync(GenerateVariationsOfImageRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadImage(request.RequestUri, request, cancellationToken);
        return result;
    }
}
