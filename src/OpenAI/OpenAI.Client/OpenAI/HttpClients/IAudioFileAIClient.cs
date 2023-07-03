using OneOf;
using OpenAI.Client.OpenAI.Models.Audio;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface IAudioFileAIClient
{

    Task<OneOf<Audio, ErrorResponse>> CreateTranscriptionsAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken);
    Task<OneOf<Audio, ErrorResponse>> CreateTranslationsAsync(AudioTranslationRequest request, CancellationToken cancellationToken);
}
