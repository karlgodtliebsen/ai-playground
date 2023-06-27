using OneOf;

using OpenAI.Client.Models.Audio;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IAudioFileAIClient
{

    Task<OneOf<Audio, ErrorResponse>> CreateTranscriptionsAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken);
    Task<OneOf<Audio, ErrorResponse>> CreateTranslationsAsync(AudioTranslationRequest request, CancellationToken cancellationToken);
}
