using AI.CaaP.AgentsDomain;
using AI.CaaP.Domain;
using AI.CaaP.WebAPI.Controllers.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.CaaP.WebAPI.Controllers;

/// <summary>
/// Conversation API Controller
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/conversation")]
[Authorize]
//[AllowAnonymous]
public class ConversationController : ControllerBase
{
    private readonly IAgentService domainService;
    private readonly IConversationService conversationService;
    private readonly IUserIdProvider userProvider;
    private readonly Serilog.ILogger logger;

    /// <summary>
    /// Controller for Chat
    /// </summary>
    /// <param name="service"></param>
    /// <param name="conversationService"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public ConversationController(IAgentService service, IConversationService conversationService, IUserIdProvider userProvider, Serilog.ILogger logger)
    {
        this.logger = logger;
        domainService = service;
        this.conversationService = conversationService;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// </summary>
    /// <param name="requests">Hold an array of Chat prompt/text</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("conversate")]
    public async Task<ConversationResponse> Converse([FromBody] ConversationRequest requests, CancellationToken cancellationToken)
    {
        var userId = userProvider.UserId;
        var conversations = new List<Conversation>();
        foreach (var textMessageRequest in requests.Prompt)
        {
            conversations.Add(new Conversation()
            {
                Content = textMessageRequest.Text!.Trim(),
                Role = ConversationRole.User.ToRole(),
                UserId = userId,
            });
        }
        var message = new ConversationMessage()
        {
            UserId = userId,
            AgentId = requests.ConversationId,
            Conversations = conversations,
            AddSystemPrompt = true,
            SystemPrompt = requests.SystemPrompt,
        };

        var conversation = await conversationService.RunConversation(message, cancellationToken);

        return new ConversationResponse()
        {
            Conversation = conversation,
        };
    }
}
