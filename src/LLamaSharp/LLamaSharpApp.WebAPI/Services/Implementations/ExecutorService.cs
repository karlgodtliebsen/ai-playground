using System.Runtime.CompilerServices;

using LLama;
using LLama.Abstractions;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Models;
using LLamaSharpApp.WebAPI.Repositories;
using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Services.Implementations;

/// <summary>
/// Executor Service
/// </summary>
public class ExecutorService : IExecutorService
{
    private readonly ILlmaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;
    private readonly InferenceOptions options;

    /// <summary>
    /// Contructor for the Executor Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="options"></param>
    public ExecutorService(ILlmaModelFactory factory, IModelStateRepository modelStateRepository, IOptions<InferenceOptions> options)
    {
        this.factory = factory;
        this.modelStateRepository = modelStateRepository;
        this.options = options.Value;
    }

    /// <summary>
    /// Activates the executor and returns the result
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<string> Executor(ExecutorInferMessage input, CancellationToken cancellationToken)
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
        var modelFileName = "./model-savedstate.st";
        var executorFileName = "./executor-savedstate.st";

        var model = factory.CreateModel();      //Default model specified by options
        var executor = LoadStatefulExecutor(input, model, modelFileName, executorFileName);

        var res = executor.InferAsync(input.Text, options, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        SaveState(input, executor, modelFileName, executorFileName);
        //model.Dispose();
    }

    private async IAsyncEnumerable<string> UseStatelessExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var model = factory.CreateModel();//Default model specified by options
        var executor = GetStatelessExecutor(model);

        var res = executor.InferAsync(input.Text, options, cancellationToken);
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

    private void SaveState(ExecutorInferMessage input, StatefulExecutorBase executor, string modelFileName, string executorFileName)
    {
        modelStateRepository.SaveState(executor.Model, () => input.UsePersistedModelState ? modelFileName : null);
        modelStateRepository.SaveState(executor, () => input.UsePersistedExecutorState ? executorFileName : null);
    }

    private StatefulExecutorBase LoadStatefulExecutor(ExecutorInferMessage input, LLamaModel model, string modelFileName, string executorFileName)
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
        modelStateRepository.LoadState(model, () => input.UsePersistedModelState ? modelFileName : null);
        modelStateRepository.LoadState(executor, () => input.UsePersistedExecutorState ? executorFileName : null);
        return executor;
    }

}
