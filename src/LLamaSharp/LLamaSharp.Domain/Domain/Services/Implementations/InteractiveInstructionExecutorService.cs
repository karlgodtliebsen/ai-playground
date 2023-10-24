using System.Runtime.CompilerServices;

using LLama;
using LLama.Abstractions;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Repositories;

namespace LLamaSharp.Domain.Domain.Services.Implementations;


/// <summary>
/// Executor Service
/// </summary>
public class InteractiveInstructionExecutorService : IInteractiveExecutorService
{
    private readonly ILLamaFactory factory;
    private readonly IContextStateRepository contextStateRepository;

    private readonly IOptionsService optionsService;
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger? msLogger = null;

    /// <summary>
    /// Constructor for the Executor Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="contextStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public InteractiveInstructionExecutorService(ILLamaFactory factory, IContextStateRepository contextStateRepository, IOptionsService optionsService, ILogger logger)
    {
        this.factory = factory;
        this.contextStateRepository = contextStateRepository;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// Activates the input parameters specified interactive executor and returns the result
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<string> Execute(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var res = input.UseStatelessExecutor ? UseStatelessExecutor(input, cancellationToken) : UseStatefulExecutor(input, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }


    public async IAsyncEnumerable<string> InteractiveExecuteInstructions(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LLamaModelOptions modelOptions = input.ModelOptions!;
        using var model = LLamaWeights.LoadFromFile(modelOptions);
        var ex = factory.CreateInteractiveExecutor(factory.CreateContext(model, modelOptions));
        await foreach (var result in ex.InferAsync(input.Prompt!, input.InferenceOptions, cancellationToken))
        {
            yield return result;
        }
    }
    public async IAsyncEnumerable<string> ExecuteInstructions(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        //LLamaWeights model, IContextParams @params, Microsoft.Extensions.Logging.ILogger? logger = null
        LLamaModelOptions modelOptions = input.ModelOptions!;
        using var model = LLamaWeights.LoadFromFile(modelOptions);

        var ex = factory.CreateInstructExecutor(factory.CreateContext(model, modelOptions));
        await foreach (var result in ex.InferAsync(input.Prompt!, input.InferenceOptions, cancellationToken))
        {
            yield return result;
        }

    }
    public async IAsyncEnumerable<string> ChatUsingInteractiveExecutor(InferenceOptions inferenceOptions, LLamaModelOptions modelOptions, string userInput,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (chatSession, _) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);
        await foreach (var result in chatSession.ChatAsync(userInput, inferenceOptions, cancellationToken))
        {
            yield return result;
        }
    }


    public async IAsyncEnumerable<string> ChatUsingInteractiveExecutorWithTransformation(InferenceOptions inferenceOptions, LLamaModelOptions modelOptions,
        KeywordTextOutputStreamTransform executionOptions, string userInput, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (chatSession, _) = factory.CreateChatSession<InteractiveExecutor>(modelOptions,
            session =>
                session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(executionOptions.Keywords, redundancyLength: executionOptions.RedundancyLength, removeAllMatchedTokens: executionOptions.RemoveAllMatchedTokens))
            );
        await foreach (var result in chatSession.ChatAsync(userInput, inferenceOptions, cancellationToken))
        {
            yield return result;
        }
    }

    private async IAsyncEnumerable<string> UseStatefulExecutor(LLamaModelOptions modelOptions, ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        //LLamaWeights model, IContextParams @params, Microsoft.Extensions.Logging.ILogger? logger = null
        using var model = LLamaWeights.LoadFromFile(modelOptions);
        var ctx = factory.CreateContext(model, modelOptions);
        var executor = LoadStatefulExecutor(input, ctx);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        var res = executor.InferAsync(input.Text!, inferenceOptions, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        SaveState(input, executor);
    }

    private async IAsyncEnumerable<string> UseStatefulExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        await foreach (var result in UseStatefulExecutor(modelOptions, input, cancellationToken).WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }

    private async IAsyncEnumerable<string> UseStatelessExecutor(LLamaModelOptions modelOptions, ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var model = LLamaWeights.LoadFromFile(modelOptions);
        var ctx = factory.CreateContext(model, modelOptions);
        var executor = GetStatelessExecutor(ctx, model);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        var res = executor.InferAsync(input.Text!, inferenceOptions, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }

    private async IAsyncEnumerable<string> UseStatelessExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        await foreach (var result in UseStatelessExecutor(modelOptions, input, cancellationToken).WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }

    private ILLamaExecutor GetStatelessExecutor(LLamaContext ctx, LLamaWeights model)
    {
        var executor = factory.CreateStateLessExecutor<StatelessExecutor>(model, ctx.Params);//maybe the future will bring several of the stateless also
        return executor;
    }

    private void SaveState(ExecutorInferMessage input, StatefulExecutorBase executor)
    {
        contextStateRepository.SaveState(executor.Context, input.UserId, input.UsePersistedModelState);
        contextStateRepository.SaveState(executor, input.UserId, input.UsePersistedExecutorState);
    }

    private StatefulExecutorBase LoadStatefulExecutor(ExecutorInferMessage input, LLamaContext ctx)
    {
        StatefulExecutorBase executor;
        switch (input.InferenceType)
        {
            case InferenceType.InteractiveExecutor:
                executor = factory.CreateStatefulExecutor<InteractiveExecutor>(ctx);
                break;
            case InferenceType.InstructExecutor:
                executor = factory.CreateStatefulExecutor<InstructExecutor>(ctx);
                break;
            default: throw new ArgumentException($"InferenceType {input.InferenceType} is not supported");
        }
        contextStateRepository.LoadState(ctx, input.UserId, input.UsePersistedModelState);
        contextStateRepository.LoadState(executor, input.UserId, input.UsePersistedExecutorState);
        return executor;
    }

}
