using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

/// <inheritdoc />
public class TokenizationService : ITokenizationService
{
    private readonly ILlmaModelFactory factory;
    private readonly IStateHandler stateHandler;

    public TokenizationService(ILlmaModelFactory factory, IStateHandler stateHandler)
    {
        this.factory = factory;
        this.stateHandler = stateHandler;
    }

    /// <summary>
    /// Get the tokens from the text
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public int[] Tokenize(TokenizeMessage input)
    {
        var fileName = "something";
        var model = factory.CreateModel();
        stateHandler.LoadState(model, () => input.UsePersistedModelState ? fileName : null);
        int[] tokens = model.Tokenize(input.Text).ToArray();
        stateHandler.SaveState(model, () => input.UsePersistedModelState ? fileName : null);
        return tokens;
    }

    /// <summary>
    /// Get the text from the tokens
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string DeTokenize(DeTokenizeMessage input)
    {
        var fileName = "something";
        var model = factory.CreateModel();
        stateHandler.LoadState(model, () => input.UsePersistedModelState ? fileName : null);
        var text = model.DeTokenize(input.Tokens);
        stateHandler.SaveState(model, () => input.UsePersistedModelState ? fileName : null);
        return text;
    }
}
