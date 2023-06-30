using System.Runtime.CompilerServices;

using LLama;
using LLama.Abstractions;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Models;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Services;

public class ExecutorService : IExecutorService
{
    private readonly ILlmaModelFactory factory;
    private readonly IStateHandler stateHandler;
    private readonly InferenceOptions options;

    public ExecutorService(ILlmaModelFactory factory, IStateHandler stateHandler, IOptions<InferenceOptions> options)
    {
        this.factory = factory;
        this.stateHandler = stateHandler;
        this.options = options.Value;
    }

    public async IAsyncEnumerable<string> Executor(ExecutorInferMessage input, CancellationToken cancellationToken)
    {
        //TODO: handle user specific parameters to override the default ones
        //TODO: how to use the difference  between Interactive and Instruct Executor?

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
        stateHandler.SaveState(executor.Model, () => input.UsePersistedModelState ? modelFileName : null);
        stateHandler.SaveState(executor, () => input.UsePersistedExecutorState ? executorFileName : null);
    }

    private StatefulExecutorBase LoadStatefulExecutor(ExecutorInferMessage input, LLamaModel model, string modelFileName, string executorFileName)
    {
        var executor = factory.CreateStatefulExecutor<InteractiveExecutor>(model);

        stateHandler.LoadState(model, () => input.UsePersistedModelState ? modelFileName : null);
        stateHandler.LoadState(executor, () => input.UsePersistedExecutorState ? executorFileName : null);
        return executor;
    }

}
