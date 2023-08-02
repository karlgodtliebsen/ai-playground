using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SerilogTimings.Extensions;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
/// Chat API Controller
/// <a href="https://scisharp.github.io/LLamaSharp/0.4/LLamaModel/parameters/">LLamaModel Parameters</a>
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
[Authorize]
//[AllowAnonymous]
public class ChatController : ControllerBase
{
    private readonly IUserIdProvider userProvider;
    private readonly ILogger logger;

    /// <summary>
    /// Controller for Chat
    /// </summary>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public ChatController(IUserIdProvider userProvider, ILogger logger)
    {
        this.logger = logger;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Invokes a chat with the prompt text, using the model parameters.
    /// </summary>
    /// <param name="request">Hold the Chat prompt/text</param>
    /// <param name="domainService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("chat")]
    public async Task<string> Chat([FromBody] ChatMessageRequest request, [FromServices] IChatDomainService domainService, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("Running Chat for {userId}...", userProvider.UserId);
        var requestModel = new ChatMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlamaModelOptions = request.LlamaModelOptions,
            UserId = userProvider.UserId
        };
        var result = await domainService.Chat(requestModel, cancellationToken);
        op.Complete();
        return result;
    }

    [HttpPost("chat/stream")]
    public async Task ChatUsingStream([FromBody] ChatMessageRequest request, [FromServices] IChatDomainService domainService, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("Running Streamed Chat for {userId}...", userProvider.UserId);

        var requestModel = new ChatMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlamaModelOptions = request.LlamaModelOptions,
            UserId = userProvider.UserId
        };

        Response.ContentType = "text/event-stream";
        await foreach (var r in domainService.ChatStream(requestModel, cancellationToken))
        {
            logger.Information("Sending SSE: {r}", r);
            await Response.WriteAsync($"data:{r}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        await Response.CompleteAsync();
        op.Complete();
    }

    //[HttpPost("History")]
    //public async Task<string> SendHistory([FromBody] HistoryInput input, [FromServices] IChatService domainService / StatelessChatService _service)
    //{
    //    var history = new ChatHistory();

    //    var messages = input.Messages.Select(m => new ChatHistory.Message(Enum.Parse<AuthorRole>(m.Role), m.Content));

    //    history.Messages.AddRange(messages);

    //    return await _service.SendAsync(history);
    //}

}
