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
    private readonly IAgentService agentService;
    private readonly ILanguageService languageService;
    private readonly IConversationRepository conversationRepository;
    private readonly ILogger logger;

    public ConversationService(IOpenAiChatCompletionService chatCompletionService, IAgentService agentService, ILanguageService languageService, IConversationRepository conversationRepository, ILogger logger)
    {
        this.chatCompletionService = chatCompletionService;
        this.agentService = agentService;
        this.languageService = languageService;
        this.conversationRepository = conversationRepository;
        this.logger = logger;
    }



    private async Task<Agent> FindAgent(Guid userId, Guid? agentId, CancellationToken cancellationToken)
    {
        Agent? agent;
        if (agentId.HasValue)
        {
            agent = await agentService.FindAgent(agentId.Value, userId, cancellationToken);
            if (agent is not null)
            {
                return agent!;
            }
        }
        agent = new Agent()
        {
            OwnerId = userId,
            Instruction = "",
            Name = "Default Agent",
        };
        agent = await agentService.CreateAgent(agent, cancellationToken);
        return agent!;
    }

    public async Task<Conversation> RunConversation(ConversationMessage message, CancellationToken cancellationToken)
    {
        string deploymentName = "gpt-3.5-turbo";
        var agent = await FindAgent(message.UserId, message.AgentId, cancellationToken);
        InitializeAgentId(agent, message);
        agent = await InitializeConversation(agent, message, cancellationToken);
        var messages = ConvertMessages(agent);
        var response = await chatCompletionService.GetChatCompletion(messages, message.UserId, deploymentName, cancellationToken);
        return await response.Match(
            async r =>
            {
                var conv = new Conversation()
                {
                    Content = r.response.Message!.Content!,
                    Role = ChatMessageRole.Assistant.AsOpenAIRole(),
                    AgentId = agent.Id,
                    UserId = message.UserId,
                };
                await conversationRepository.AddConversation(conv, cancellationToken);
                return conv;
            },
            error => throw new AIException(error.Error)
        );
    }

    private void InitializeAgentId(Agent agent, ConversationMessage message)
    {
        //check if the conversation is already started
        if (!message.AgentId.HasValue)
        {
            message.AgentId = agent.Id;
        }
    }

    private async Task<Agent> InitializeConversation(Agent agent, ConversationMessage message, CancellationToken cancellationToken)
    {
        if (!message.AgentId.HasValue)
        {
            throw new AIException("AgentId is required");
        }
        Guid agentId = message.AgentId.Value;
        //check if the conversation is already started
        IList<Conversation> existingConversations = await conversationRepository.GetConversationsByAgentId(agentId, cancellationToken);
        if (existingConversations.Any(c => c.UserId != message.UserId))
        {
            throw new AIException($"Detected conversation having non matching UserId {message.UserId} for Conversation/Agent {agentId}");
        }

        var conversations = message.Conversations;
        foreach (var conversation in conversations)
        {
            conversation.AgentId = agentId;
        }
        AddSystemPrompt(agent, message, existingConversations, conversations);
        //persist the new set of conversation
        await conversationRepository.AddConversation(conversations, cancellationToken);
        var messages = new List<Conversation>();
        foreach (var conv in existingConversations)
        {
            messages.Add(conv);
        }
        foreach (var conv in conversations)
        {
            messages.Add(conv);
        }
        agent.Messages = messages;
        return agent;
    }

    private void AddSystemPrompt(Agent agent, ConversationMessage message, IList<Conversation> existingConversations, IList<Conversation> conversations)
    {
        if (message.AddSystemPrompt && !existingConversations.Any() && conversations.First().Role != ChatMessageRole.System.AsOpenAIRole())
        {
            var conversation = languageService.GetPromptByIndex(message.SystemPrompt);
            if (conversation is not null)
            {
                conversation.UserId = message.UserId;
                conversation.AgentId = agent.Id;
                conversations.Insert(0, conversation);
            }
        }
    }

    private List<ChatCompletionMessage> ConvertMessages(Agent agent)
    {
        var messages = new List<ChatCompletionMessage>();

        foreach (var conv in agent.Messages)
        {
            messages.Add(new ChatCompletionMessage
            {
                Role = conv.Role,
                Content = conv.Content
            });
        }
        return messages;
    }
}
