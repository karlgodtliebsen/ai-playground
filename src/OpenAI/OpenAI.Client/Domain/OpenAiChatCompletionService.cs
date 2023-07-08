using OneOf;

using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.Chat;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.Domain;

public class OpenAiChatCompletionService : IOpenAiChatCompletionService
{
    private readonly IChatCompletionAIClient aiClient;
    private readonly ILogger logger;
    private readonly IModelRequestFactory requestFactory;

    /// <summary>
    /// System prompt to send with user prompts to instruct the model for chat session
    /// </summary>
    private readonly string systemPrompt = @"
        You are an AI assistant that helps people find information.
        Provide concise answers that are polite and professional." + Environment.NewLine;

    /// <summary>    
    /// System prompt to send with user prompts to instruct the model for summarization
    /// </summary>
    private readonly string summarizePrompt = @"Summarize this prompt in one or two words to use as a label in a button on a web page" + Environment.NewLine;


    public OpenAiChatCompletionService(IChatCompletionAIClient aiClient, IModelRequestFactory requestFactory, ILogger logger)
    {
        this.aiClient = aiClient;
        this.logger = logger;
        this.requestFactory = requestFactory;
    }

    public void AddStandardSystemPrompt(IList<ChatCompletionMessage> messages)
    {
        messages.Insert(0, new ChatCompletionMessage
        {
            Role = ChatMessageRole.System.AsOpenAIRole(),
            Content = systemPrompt
        });
    }

    public void AddSummarizePrompt(IList<ChatCompletionMessage> messages)
    {
        messages.Insert(0, new ChatCompletionMessage
        {
            Role = ChatMessageRole.System.AsOpenAIRole(),
            Content = summarizePrompt
        });
    }


    /// <summary>
    /// Sends a prompt to the deployed OpenAI LLM model and returns the response.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="userPrompt">Prompt message to send to the deployment.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response from the OpenAI model along with tokens for the prompt and response.</returns>
    public async Task<(string response, int promptTokens, int responseTokens)> GetChatCompletion(Guid sessionId, string userPrompt, CancellationToken cancellationToken)
    {

        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role =ChatMessageRole.System.AsOpenAIRole(), Content = systemPrompt },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = userPrompt },
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages,
                Temperature = 0.3f,
                MaxTokens = 4000,
                User = sessionId.ToString("N"),
                TopP = 0.5f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            });

        var response = await aiClient.GetChatCompletionsAsync(payload, cancellationToken);
        return response.Match(
            completions =>
            {
                var result = completions.Choices.First().Message!.Content!;
                var promptTokens = completions.Usage.PromptTokens;
                var responseTokens = completions.Usage.CompletionTokens;
                return (response: result, promptTokens: promptTokens, responseTokens: responseTokens);
            },
            error => throw new AIException(error.Error)
        );
    }


    /// <summary>
    /// Sends a prompt to the OpenAI LLM model and returns the response.
    /// </summary>
    /// <param name="messages">Collection of  ChatCompletionMessage</param>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="deploymentName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response from the OpenAI model along with tokens for the prompt and response.</returns>
    public async Task<OneOf<(ChatChoice response, int promptTokens, int responseTokens), ErrorResponse>> GetChatCompletion(
    IList<ChatCompletionMessage> messages,
    Guid sessionId, string deploymentName, CancellationToken cancellationToken)
    {

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages.ToArray(),
                Temperature = 0.3f,
                MaxTokens = 4000,
                User = sessionId.ToString("N"),
                TopP = 0.5f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            });

        var response = await aiClient.GetChatCompletionsAsync(payload, cancellationToken);
        return response.Match(
            completions =>
            {
                var result = completions.Choices.First();
                var promptTokens = completions.Usage.PromptTokens;
                var responseTokens = completions.Usage.CompletionTokens;
                return (response: result, promptTokens: promptTokens, responseTokens: responseTokens);
            },
            error => throw new AIException(error.Error)
        );
    }

    /// <summary>
    /// Sends the existing conversation to the OpenAI model and returns a two word summary.
    /// </summary>
    /// <param name="sessionId">Chat session identifier for the current conversation.</param>
    /// <param name="conversation">Prompt conversation to send to the deployment.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Summarization response from the OpenAI model deployment.</returns>
    public async Task<string> Summarize(Guid sessionId, string userPrompt, CancellationToken cancellationToken)
    {
        string deploymentName = "gpt-3.5-turbo";
        var messages = new[]
        {
            new ChatCompletionMessage {Role = ChatMessageRole.System.AsOpenAIRole(), Content = summarizePrompt },
            new ChatCompletionMessage { Role = ChatMessageRole.User.AsOpenAIRole(), Content = userPrompt },
        };

        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = deploymentName,
                Messages = messages,
                Temperature = 0.0f,
                MaxTokens = 200,
                User = sessionId.ToString("N"),
                TopP = 0.5f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            });

        var response = await aiClient.GetChatCompletionsAsync(payload, cancellationToken);
        return response.Match(
            completions =>
            {
                string summary = completions.Choices.First().Message!.Content!;
                return summary;
            },
            error => throw new AIException(error.Error)
        );
    }
}
