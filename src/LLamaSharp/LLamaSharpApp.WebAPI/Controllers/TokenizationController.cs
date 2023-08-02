using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
///Tokenization Controller
/// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/LLamaModel/tokenization.md"/>
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/llama")]
[Authorize]
public class TokenizationController : ControllerBase
{
    private readonly IUserIdProvider userProvider;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for TokenizationController
    /// </summary>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public TokenizationController(IUserIdProvider userProvider, ILogger logger)
    {
        this.logger = logger;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Tokenize a text
    /// </summary>
    /// <param name="request"></param>
    /// <param name="domainService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("tokenize")]
    public async Task<int[]> Tokenize([FromBody] TokenizeMessageRequest request, [FromServices] ITokenizationService domainService, CancellationToken cancellationToken)
    {
        var requestModel = new TokenizeMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlamaModelOptions = request.LlamaModelOptions,
            UserId = userProvider.UserId
        };
        return await domainService.Tokenize(requestModel, cancellationToken);
    }

    /// <summary>
    /// DeTokenize an array of tokens to create a text
    /// </summary>
    /// <param name="request"></param>
    /// <param name="domainService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("detokenize")]
    public async Task<string> DeTokenize([FromBody] DeTokenizeMessageRequest request, [FromServices] ITokenizationService domainService, CancellationToken cancellationToken)
    {
        var requestModel = new DeTokenizeMessage(request.Tokens)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlamaModelOptions = request.LlamaModelOptions,
            UserId = userProvider.UserId
        };
        return await domainService.DeTokenize(requestModel, cancellationToken);
    }
}
