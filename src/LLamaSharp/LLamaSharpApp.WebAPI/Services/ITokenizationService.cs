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
    /// <returns></returns>
    int[] Tokenize(TokenizeMessage input);

    /// <summary>
    /// Get the text for the specified tokens
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string DeTokenize(DeTokenizeMessage input);
}
