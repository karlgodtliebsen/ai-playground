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
    [HttpPost("converse")]
    public async Task Converse([FromBody] TextMessageRequest[] requests, CancellationToken cancellationToken)
    {
        var userId = userProvider.UserId;

        var agent = await domainService.FindAgent(userId, cancellationToken);
        if (agent == null)
        {
            agent = new Agent()
            {
                OwnerId = userId,
                Instruction = "",
                Name = "Default Agent",
            };
            agent = await domainService.CreateAgent(agent, cancellationToken);
        }

        var conversations = new List<Conversation>();
        foreach (var textMessageRequest in requests)
        {
            conversations.Add(new Conversation()
            {
                Content = textMessageRequest.Text!.Trim(),
                AgentId = agent.Id,
                Role = ConversationRole.User.ToString(),
                UserId = userId
            });
        }
        await conversationService.ExecuteConversation(userId, agent, conversations, cancellationToken);
    }
}
