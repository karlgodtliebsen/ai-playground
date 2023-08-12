﻿using System.Runtime.CompilerServices;

using LLama;
using LLama.Abstractions;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Repositories;

namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <summary>
/// Executor Service
/// </summary>
public class ExecutorService : IExecutorService
{
    private readonly ILlamaModelFactory factory;
    private readonly IModelStateRepository modelStateRepository;

    private readonly IOptionsService optionsService;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor for the Executor Service
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="modelStateRepository"></param>
    /// <param name="optionsService"></param>
    /// <param name="logger"></param>
    public ExecutorService(ILlamaModelFactory factory, IModelStateRepository modelStateRepository, IOptionsService optionsService, ILogger logger)
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
        var res = input.UseStatelessExecutor ? UseStatelessExecutor(input, cancellationToken) : UseStatefulExecutor(input, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }

    public IEnumerable<string> ChatUsingInteractiveExecutor(InferenceOptions inferenceOptions, LlamaModelOptions modelOptions, string userInput)
    {
        var (chatSession, _) = factory.CreateChatSession<InteractiveExecutor>(modelOptions);
        var outputs = chatSession.Chat(userInput, inferenceOptions);
        foreach (var output in outputs)
        {
            yield return output;
        }
    }

    public IEnumerable<string> ExecutorWithTransformation(InferenceOptions inferenceOptions, LlamaModelOptions modelOptions, string userInput)
    {
        //LLamaModel
        var (chatSession, _) = factory.CreateChatSession<InteractiveExecutor>(modelOptions,
            (session =>
                session.WithOutputTransform(
                    new LLamaTransforms.KeywordTextOutputStreamTransform(
                    new string[] { "User:", "Bob:" },
                    redundancyLength: 8))));

        //InteractiveExecutor ex = new(new LLamaModel(new ModelParams(modelPath, contextSize: 1024, seed: 1337, gpuLayerCount: 5)));
        //ChatSession session = new ChatSession(ex); // The only change is to remove the transform for the output text stream.
        //foreach (var text in session.Chat(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))

        var outputs = chatSession.Chat(userInput, inferenceOptions);

        foreach (var output in outputs)
        {
            yield return output;
        }
    }

    private async IAsyncEnumerable<string> UseStatefulExecutor(LlamaModelOptions modelOptions, ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var model = factory.CreateModel(modelOptions);
        var executor = LoadStatefulExecutor(input, model);
        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        var res = executor.InferAsync(input.Text, inferenceOptions, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        SaveState(input, executor);
        //model.Dispose();  //TODO: must be solved
    }

    private async IAsyncEnumerable<string> UseStatefulExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        await foreach (var result in UseStatefulExecutor(modelOptions, input, cancellationToken).WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }

    private async IAsyncEnumerable<string> UseStatelessExecutor(LlamaModelOptions modelOptions, ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var model = factory.CreateModel(modelOptions);//model specified by options
        var executor = GetStatelessExecutor(model);

        var inferenceOptions = await optionsService.GetInferenceOptions(input.UserId, cancellationToken);
        var res = executor.InferAsync(input.Text, inferenceOptions, cancellationToken);
        await foreach (var result in res.WithCancellation(cancellationToken))
        {
            yield return result;
        }
        //model.Dispose();  //TODO: must be solved
    }

    private async IAsyncEnumerable<string> UseStatelessExecutor(ExecutorInferMessage input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var modelOptions = await optionsService.GetLlamaModelOptions(input.UserId, cancellationToken);
        await foreach (var result in UseStatelessExecutor(modelOptions, input, cancellationToken).WithCancellation(cancellationToken))
        {
            yield return result;
        }
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
