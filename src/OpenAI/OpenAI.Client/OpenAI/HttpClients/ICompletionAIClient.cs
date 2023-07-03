using OneOf;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients;

public interface ICompletionAIClient
{
    Task<OneOf<Completions, ErrorResponse>> GetCompletionsAsync(CompletionRequest request, CancellationToken none);

    IAsyncEnumerable<OneOf<Completions, ErrorResponse>> GetCompletionsStreamAsync(CompletionRequest request, CancellationToken cancellationToken);

}
