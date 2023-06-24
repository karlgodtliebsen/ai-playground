using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models.Audio;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

using SerilogTimings.Extensions;

using System.Net.Http.Json;

namespace OpenAI.Client.AIClients.Implementation;

public class AudioFileAIClient : AIClientBase, IAudioFileAIClient
{

    public AudioFileAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    private async Task<Audio?> UploadAudioFileAsync(string subUri, AudioTranscriptionRequest request, CancellationToken cancellationToken)
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
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UploadAudioFileAsync Failed {uri}", subUri);
        }

        return default;
    }

    private async Task<Audio?> UploadAudioFileAsync(string subUri, AudioTranslationRequest request, CancellationToken cancellationToken)
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
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UploadAudioFileAsync Failed {uri}", subUri);
        }

        return default;
    }

    public async Task<Response<Audio>> CreateTranscriptionsAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadAudioFileAsync(request.RequestUri, request, cancellationToken);
        return new Response<Audio>(result!);
    }

    public async Task<Response<Audio>> CreateTranslationsAsync(AudioTranslationRequest request, CancellationToken cancellationToken)
    {
        var result = await UploadAudioFileAsync(request.RequestUri, request, cancellationToken);
        return new Response<Audio>(result!);
    }
}