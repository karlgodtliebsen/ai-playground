using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OneOf;
using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.Audio;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;
using SerilogTimings.Extensions;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class AudioFileAIClient : AIClientBase, IAudioFileAIClient
{

    public AudioFileAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIConfiguration> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    private async Task<OneOf<Audio, ErrorResponse>> UploadAudioFileAsync(string subUri, AudioTranscriptionRequest request, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("UploadAudioFileAsync", subUri);
        try
        {
            PrepareClient();
            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Model), "model" },
                { new StringContent(request.ResponseFormat), "response_format" },
                { new StringContent(request.Prompt), "prompt" },
                { new StringContent(request.Temperature!.ToString()) , "temperature" },
                { new StringContent(request.Language!) , "language" },

                { new ByteArrayContent(await File.ReadAllBytesAsync(request.FullFilename, cancellationToken)), "file", Path.GetFileName(request.FullFilename) }
            };
            var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Audio>(cancellationToken: cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UploadAudioFileAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    private async Task<OneOf<Audio, ErrorResponse>> UploadAudioFileAsync(string subUri, AudioTranslationRequest request, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("UploadAudioFileAsync", subUri);
        try
        {
            PrepareClient();
            var content = new MultipartFormDataContent
            {
                { new StringContent(request.Model), "model" },
                { new StringContent(request.ResponseFormat), "response_format" },
                { new StringContent(request.Prompt), "prompt" },
                { new StringContent(request.Temperature!.ToString()) , "temperature" },
                { new ByteArrayContent(await File.ReadAllBytesAsync(request.FullFilename, cancellationToken)), "file", Path.GetFileName(request.FullFilename) }
            };
            var response = await HttpClient.PostAsync(subUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Audio>(cancellationToken: cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UploadAudioFileAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    public async Task<OneOf<Audio, ErrorResponse>> CreateTranscriptionsAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadAudioFileAsync(request.RequestUri, request, cancellationToken);
        return result;
    }

    public async Task<OneOf<Audio, ErrorResponse>> CreateTranslationsAsync(AudioTranslationRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadAudioFileAsync(request.RequestUri, request, cancellationToken);
        return result;
    }
}
