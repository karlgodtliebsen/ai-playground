using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;

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
public class TokenizationController : ControllerBase
{
    private readonly ITokenizationService domainService;
    private readonly IUserIdProvider userProvider;
    private readonly ILogger<TokenizationController> logger;

    /// <summary>
    /// Construcor for TokenizationController
    /// </summary>
    /// <param name="service"></param>
    /// <param name="userProvider"></param>
    /// <param name="logger"></param>
    public TokenizationController(ITokenizationService service, IUserIdProvider userProvider, ILogger<TokenizationController> logger)
    {
        this.logger = logger;
        domainService = service;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Tokenize a text
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("tokenize")]
    public async Task<int[]> Tokenizen([FromBody] TokenizeMessageRequest request, CancellationToken cancellationToken)
    {
        var requestModel = new TokenizeMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlmaModelOptions = request.LlmaModelOptions,
            UserId = userProvider.UserId
        };
        return await domainService.Tokenize(requestModel, cancellationToken);
    }

    /// <summary>
    /// DeTokenize an array of tokens to create a text
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("detokenize")]
    public async Task<string> DeTokenize([FromBody] DeTokenizeMessageRequest request, CancellationToken cancellationToken)
    {
        var requestModel = new DeTokenizeMessage(request.Tokens)
        {
            UsePersistedModelState = request.UsePersistedModelState,
            LlmaModelOptions = request.LlmaModelOptions,
            UserId = userProvider.UserId
        };
        return await domainService.DeTokenize(requestModel, cancellationToken);
    }
}
