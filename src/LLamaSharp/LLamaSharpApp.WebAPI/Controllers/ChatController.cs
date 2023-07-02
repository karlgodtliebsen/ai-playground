using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Chat API Controller
/// <a href="https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/parameters/"/>
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
//[Authorize]
//[AllowAnonymous]
public class ChatController : ControllerBase
{
    private readonly IChatService domainService;
    private readonly IUserIdProvider userProvider;
    private readonly ILogger<ChatController> logger;

    /// <summary>
    /// Controller for Chat
    /// </summary>
    /// <param name="service"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public ChatController(IChatService service, IUserIdProvider userProvider, ILogger<ChatController> logger)
    {
        this.logger = logger;
        domainService = service;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("chat")]
    public async Task<string> Chat([FromBody] ChatMessageRequest request, CancellationToken cancellationToken)
    {
        var requestModel = new ChatMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlmaModelOptions = request.LlmaModelOptions,
            UserId = userProvider.UserId
        };
        return await domainService.Chat(requestModel, cancellationToken);
    }
}
