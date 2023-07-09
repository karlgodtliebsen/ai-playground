using System.Runtime.CompilerServices;

using LLama;
using LLama.Abstractions;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Repositories;
using LLamaSharpApp.WebAPI.Domain.Services;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Executor Service
/// </summary>
public class ExecutorService : IExecutorService
{
    private readonly ILlamaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;

    private readonly IOptionsService optionsService;
    private readonly ILogger<ExecutorService> logger;

    /// <summary>
    /// Constructor for the Executor Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public ExecutorService(ILlamaModelFactory factory, IModelStateRepository modelStateRepository, IOptionsService optionsService, ILogger<ExecutorService> logger)
    {
        this.factory = factory;
        this.modelStateRepository = modelStateRepository;
        this.optionsService = optionsService;
        this.logger = logger;
    }

    /// <summary>
    /// Activates the executor and returns the result
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<string> Executor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerable<string> res;
        if (input.UseStatelessExecutor)
        {
            res = UseStatelessExecutor(input, cancellationToken);
        }
        else
        {
            res = UseStatefulExecutor(input, cancellationToken);
        }
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }

    private async IAsyncEnumerable<string> UseStatefulExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);

        var model = factory.CreateModel(modelOptions);      //model specified by options
        var executor = LoadStatefulExecutor(input, model);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        var res = executor.InferAsync(input.Text, inferenceOptions, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        SaveState(input, executor);
        //model.Dispose();
    }

    private async IAsyncEnumerable<string> UseStatelessExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);

        var model = factory.CreateModel(modelOptions);//model specified by options
        var executor = GetStatelessExecutor(model);

        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        var res = executor.InferAsync(input.Text, inferenceOptions, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        //model.Dispose();
    }

    private ILLamaExecutor GetStatelessExecutor(LLamaModel model)
    {
        var executor = factory.CreateStateLessExecutor<StatelessExecutor>(model);//maybe the future will bring several of the stateless also
        return executor;
    }

    private void SaveState(ExecutorInferMessage input, StatefulExecutorBase executor)
    {
        modelStateRepository.SaveState(executor.Model, input.UserId, input.UsePersistedModelState);
        modelStateRepository.SaveState(executor, input.UserId, input.UsePersistedExecutorState);
    }

    private StatefulExecutorBase LoadStatefulExecutor(ExecutorInferMessage input, LLamaModel model)
    {
        StatefulExecutorBase executor;
        switch (input.InferenceType)
        {
            case InferenceType.InteractiveExecutor:
                executor = factory.CreateStatefulExecutor<InteractiveExecutor>(model);
                break;
            case InferenceType.InstructExecutor:
                executor = factory.CreateStatefulExecutor<InstructExecutor>(model);
                break;
            default: throw new ArgumentException($"InferenceType {input.InferenceType} is not supported");
        }
        modelStateRepository.LoadState(model, input.UserId, input.UsePersistedModelState);
        modelStateRepository.LoadState(executor, input.UserId, input.UsePersistedExecutorState);
        return executor;
    }

}
