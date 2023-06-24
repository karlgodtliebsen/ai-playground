using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface ICompletionAIClient
{
    Task<Response<Completions>?> GetCompletionsAsync(CompletionRequest request, CancellationToken none);

}
