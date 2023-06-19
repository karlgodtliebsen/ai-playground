using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;
using SerilogTimings.Extensions;

namespace OpenAI.Client.AIClients.Implementation;

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

    private async Task<GeneratedImage> UploadImage(string subUri, GenerateVariationsOfImageRequest request, CancellationToken cancellationToken = default)
    {
        using var op = logger.BeginOperation("UploadImageAsync for Variations", subUri);
        using var content = new MultipartFormDataContent();
        using var imageData = new MemoryStream();
        await request.ImageStream.CopyToAsync(imageData, cancellationToken).ConfigureAwait(false);
        content.Add(new ByteArrayContent(imageData.ToArray()), "image", request.Image);
        content.Add(new StringContent(request.NumberOfImagesToGenerate.ToString()), "n");
        content.Add(new StringContent(request.Size), "size");
        content.Add(new StringContent(request.ResponseFormat), "response_format");

        if (!string.IsNullOrWhiteSpace(request.User))
        {
            content.Add(new StringContent(request.User), "user");
        }
        PrepareClient();
        var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GeneratedImage>(cancellationToken: cancellationToken);
        op.Complete();
        return result!;
    }

    private async Task<GeneratedImage> UploadImage(string subUri, GenerateEditedImageRequest request, CancellationToken cancellationToken = default)
    {
        using var op = logger.BeginOperation("UploadImageAsync For Edit", subUri);
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
            content.Add(new StringContent(request.User), "user");
        }
        PrepareClient();
        var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GeneratedImage>(cancellationToken: cancellationToken);
        op.Complete();
        return result!;
    }
    public async Task<Response<GeneratedImage>?> CreateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<ImageGenerationRequest, GeneratedImage>("images/generations", request, cancellationToken);
        return new Response<GeneratedImage>(result!);
    }

    public async Task<Response<GeneratedImage>?> CreateImageEditsAsync(GenerateEditedImageRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadImage("images/edits", request, cancellationToken);
        return new Response<GeneratedImage>(result!);
    }
    public async Task<Response<GeneratedImage>?> CreateImageVariationsAsync(GenerateVariationsOfImageRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadImage("images/variations", request, cancellationToken);
        return new Response<GeneratedImage>(result!);
    }
}