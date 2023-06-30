using System.ComponentModel;

using LLamaSharpApp.WebAPI.Controllers.Requests;
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
[DisplayName("Llma Chat Controller <a href=\"https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/parameters/\">")]
[Description("API to execute Chat using model parameters.")]
[ApiController]
[Route("api/llama")]
public class ChatController : ControllerBase
{
    private readonly IChatService domainService;
    private readonly ILogger<ChatController> logger;

    public ChatController(IChatService service, ILogger<ChatController> logger)
    {
        this.logger = logger;
        domainService = service;
    }


    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <returns></returns>
    [HttpPost("chat")]
    public string Chat([FromBody] ChatMessageRequest request)
    {
        var requestModel = new ChatMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState
        };
        return domainService.Chat(requestModel);
    }
}
