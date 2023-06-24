using OpenAI.Client.Models.Audio;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IAudioFileAIClient
{

    Task<Response<Audio>> CreateTranscriptionsAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken);
    Task<Response<Audio>> CreateTranslationsAsync(AudioTranslationRequest request, CancellationToken cancellationToken);
}