using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

/// <summary>
/// Tokenization service
/// </summary>
public interface ITokenizationService
{
    /// <summary>
    /// Get the tokesn for the specified text
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int[]> Tokenize(TokenizeMessage input, CancellationToken cancellationToken);

    /// <summary>
    /// Get the text for the specified tokens
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> DeTokenize(DeTokenizeMessage input, CancellationToken cancellationToken);
}
