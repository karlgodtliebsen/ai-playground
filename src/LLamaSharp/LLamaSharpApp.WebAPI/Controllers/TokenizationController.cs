using System.ComponentModel;

using LLamaSharpApp.WebAPI.Controllers.Requests;
using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace LLamaSharpApp.WebAPI.Controllers;

/// <summary>
///Tokenization Controller
///  <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/LLamaModel/tokenization.md"/>
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[DisplayName("Llma  Tokenize Controller <a href=\"https://github.com/SciSharp/LLamaSharp/blob/master/docs/LLamaModel/tokenization.md\">")]
[Description("API to create tokens from text and text from tokens.")]
//[Authorize] Introduced later on
[ApiController]
[Route("api/llama")]
public class TokenizationController : ControllerBase
{
    private readonly ITokenizationService domainService;
    private readonly ILogger<TokenizationController> logger;

    public TokenizationController(ITokenizationService service, ILogger<TokenizationController> logger)
    {
        this.logger = logger;
        domainService = service;
    }


    /// <summary>
    /// Tokenize a text
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("tokenize")]
    public int[] Tokenizen([FromBody] TokenizeMessageRequest request)
    {
        var requestModel = new TokenizeMessage(request.Text)
        {
            UsePersistedModelState = request.UsePersistedModelState
        };
        return domainService.Tokenize(requestModel);
    }

    /// <summary>
    /// DeTokenize an array of tokens to create a text
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("detokenize")]
    public string DeTokenize([FromBody] DeTokenizeMessageRequest request)
    {
        var requestModel = new DeTokenizeMessage(request.Tokens)
        {
            UsePersistedModelState = request.UsePersistedModelState
        };
        return domainService.DeTokenize(requestModel);
    }
}
