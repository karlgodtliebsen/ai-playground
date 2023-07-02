using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Repositories;

namespace LLamaSharpApp.WebAPI.Services.Implementations;

/// <inheritdoc />
public class TokenizationService : ITokenizationService
{
    private readonly ILlmaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;
    private readonly IOptionsService optionsService;
    private readonly ILogger<TokenizationService> logger;

    /// <summary>
    /// Constructor for Tokenization Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public TokenizationService(ILlmaModelFactory factory, IModelStateRepository modelStateRepository, IOptionsService optionsService, ILogger<TokenizationService> logger)
    {
        this.factory = factory;
        this.modelStateRepository = modelStateRepository;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// Get the tokens from the text
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int[]> Tokenize(TokenizeMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlmaModelOptions(input.UserId, cancellationToken);
        var model = factory.CreateModel(modelOptions);
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var tokens = model.Tokenize(input.Text).ToArray();
        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        return tokens;
    }

    /// <summary>
    /// Get the text from the tokens
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> DeTokenize(DeTokenizeMessage input, CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlmaModelOptions(input.UserId, cancellationToken);
        var model = factory.CreateModel(modelOptions);
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var text = model.DeTokenize(input.Tokens);
        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        return text;
    }
}
