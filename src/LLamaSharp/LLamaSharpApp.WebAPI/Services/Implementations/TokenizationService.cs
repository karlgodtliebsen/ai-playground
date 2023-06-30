using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Repositories;

namespace LLamaSharpApp.WebAPI.Services.Implementations;

/// <inheritdoc />
public class TokenizationService : ITokenizationService
{
    private readonly ILlmaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;

    public TokenizationService(ILlmaModelFactory factory, IModelStateRepository modelStateRepository)
    {
        this.factory = factory;
        this.modelStateRepository = modelStateRepository;
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
        modelStateRepository.LoadState(model, () => input.UsePersistedModelState ? fileName : null);
        var tokens = model.Tokenize(input.Text).ToArray();
        modelStateRepository.SaveState(model, () => input.UsePersistedModelState ? fileName : null);
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
        modelStateRepository.LoadState(model, () => input.UsePersistedModelState ? fileName : null);
        var text = model.DeTokenize(input.Tokens);
        modelStateRepository.SaveState(model, () => input.UsePersistedModelState ? fileName : null);
        return text;
    }
}
