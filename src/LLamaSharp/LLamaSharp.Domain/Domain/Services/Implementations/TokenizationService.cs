using LLama;

using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Repositories;

using SerilogTimings.Extensions;

namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <inheritdoc />
public class TokenizationService : ITokenizationService
{
    private readonly ILLamaFactory factory;
    private readonly IContextStateRepository contextStateRepository;
    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for Tokenization Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="contextStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public TokenizationService(ILLamaFactory factory, IContextStateRepository contextStateRepository, IOptionsService optionsService, ILogger logger)
    {
        this.factory = factory;
        this.contextStateRepository = contextStateRepository;
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
        //var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var modelOptions = await optionsService.CoalsceLlamaModelOptions(input.ModelOptions, input.UserId, cancellationToken);
        using var model = LLamaWeights.LoadFromFile(modelOptions);
        var ctx = factory.CreateContext(model, modelOptions);
        contextStateRepository.LoadState(ctx, input.UserId, input.UsePersistedModelState);
        var tokens = ctx.Tokenize(input.Text).ToArray();
        contextStateRepository.SaveState(ctx, input.UserId, input.UsePersistedModelState);
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
        //var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        var modelOptions = await optionsService.CoalsceLlamaModelOptions(input.ModelOptions, input.UserId, cancellationToken);
        using var model = LLamaWeights.LoadFromFile(modelOptions);
        var ctx = factory.CreateContext(model, modelOptions);
        contextStateRepository.LoadState(ctx, input.UserId, input.UsePersistedModelState);
        var text = ctx.DeTokenize(input.Tokens);
        contextStateRepository.SaveState(ctx, input.UserId, input.UsePersistedModelState);
        op.Complete();
        return text;
    }
}
