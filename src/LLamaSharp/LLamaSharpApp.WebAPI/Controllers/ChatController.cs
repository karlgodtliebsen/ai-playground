using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService domainService;
    private readonly ILogger<ChatController> logger;

    public ChatController(ILogger<ChatController> logger,
        IChatService service)
    {
        this.logger = logger;
        domainService = service;
    }


    [HttpPost("chat")]
    public string SendMessage([FromBody] SendMessageRequest request)
    {
        var requestModel = new SendMessage(request.Text);
        return domainService.Send(requestModel);
    }
}
