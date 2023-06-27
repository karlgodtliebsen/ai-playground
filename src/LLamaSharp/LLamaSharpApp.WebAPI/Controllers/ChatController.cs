using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _service;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ILogger<ChatController> logger,
        IChatService service)
    {
        _logger = logger;
        _service = service;
    }


    [HttpPost("Send")]
    public string SendMessage([FromBody] SendMessageInput input)
    {
        return _service.Send(input);
    }
}
