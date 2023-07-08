using AI.CaaP.AgentsDomain;
using AI.CaaP.Repositories;

using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.Models.Chat;
using OpenAI.Client.OpenAI.Models.ChatCompletion;

using Serilog;

namespace AI.CaaP.Domain;

public class ConversationService : IConversationService
{
    private readonly IOpenAiChatCompletionService chatCompletionService;
    private readonly IConversationRepository conversationRepository;
    private readonly ILogger logger;

    public ConversationService(IOpenAiChatCompletionService chatCompletionService, IConversationRepository conversationRepository, ILogger logger)
    {
        this.chatCompletionService = chatCompletionService;
        this.conversationRepository = conversationRepository;
        this.logger = logger;
    }


    public async Task<Conversation> ExecuteConversation(Guid userId, Agent agent, IList<Conversation> conversations, CancellationToken cancellationToken)
    {
        string deploymentName = "gpt-3.5-turbo";
        var messages = new List<ChatCompletionMessage>();

        var allConversations = await conversationRepository.GetConversationsByUserId(userId, CancellationToken.None);
        foreach (var conv in allConversations)
        {
            messages.Add(new ChatCompletionMessage { Role = conv.Role, Content = conv.Content });
        }

        foreach (var conv in conversations)
        {
            messages.Add(new ChatCompletionMessage { Role = conv.Role, Content = conv.Content });
        }
        //persist the conversation
        foreach (var conv in conversations)
        {
            await conversationRepository.AddConversation(conv, cancellationToken);
        }

        var response = await chatCompletionService.GetChatCompletion(messages.ToList(), userId, deploymentName, cancellationToken);
        return await response.Match(
            async r =>
            {
                var conv = new Conversation()
                {
                    AgentId = agent.Id,
                    Content = r.response.Message!.Content!,
                    Role = ChatMessageRole.Assistent.ToString(),
                    UserId = userId
                };
                await conversationRepository.AddConversation(conv, cancellationToken);
                return conv;
            },
            error => throw new AIException(error.Error)
        );
    }

}
