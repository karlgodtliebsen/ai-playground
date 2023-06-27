using OneOf;

using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface ICompletionAIClient
{
    Task<OneOf<Completions, ErrorResponse>> GetCompletionsAsync(CompletionRequest request, CancellationToken none);

    IAsyncEnumerable<OneOf<Completions, ErrorResponse>> GetCompletionsStreamAsync(CompletionRequest request, CancellationToken cancellationToken);

}
