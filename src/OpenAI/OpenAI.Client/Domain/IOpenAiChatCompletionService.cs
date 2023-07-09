using OneOf;

using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.Domain;

public interface IOpenAiChatCompletionService
{
    /// <summary>
    /// Sends a prompt to the deployed OpenAI LLM model and returns the response.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="userPrompt">Prompt message to send to the deployment.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response from the OpenAI model along with tokens for the prompt and response.</returns>
    Task<(string response, int promptTokens, int responseTokens)> GetChatCompletion(Guid sessionId, string userPrompt, CancellationToken cancellationToken);

    /// <summary>
    /// Sends the existing conversation to the OpenAI model and returns a two word summary.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="userPrompt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Summarization response from the OpenAI model deployment.</returns>
    Task<string> Summarize(Guid sessionId, string userPrompt, CancellationToken cancellationToken);


    Task<OneOf<(ChatChoice response, int promptTokens, int responseTokens), ErrorResponse>> GetChatCompletion(IList<ChatCompletionMessage> messages, Guid sessionId, string deploymentName, CancellationToken cancellationToken);
}
