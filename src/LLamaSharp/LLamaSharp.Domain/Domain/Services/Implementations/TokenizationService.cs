using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharpApp.WebAPI.Domain.Models;
using SerilogTimings.Extensions;

namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <inheritdoc />
public class TokenizationService : ITokenizationService
{
    private readonly ILlamaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for Tokenization Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public TokenizationService(ILlamaModelFactory factory, IModelStateRepository modelStateRepository, IOptionsService optionsService, ILogger logger)
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
        using var op = logger.BeginOperation("Running Tokenize");

        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var model = factory.CreateModel(modelOptions);
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var tokens = model.Tokenize(input.Text).ToArray();
        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        op.Complete();
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
        using var op = logger.BeginOperation("Running DeTokenize");
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var model = factory.CreateModel(modelOptions);
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        var text = model.DeTokenize(input.Tokens);
        modelStateRepository.SaveState(model, input.UserId, input.UsePersistedModelState);
        op.Complete();
        return text;
    }
}
