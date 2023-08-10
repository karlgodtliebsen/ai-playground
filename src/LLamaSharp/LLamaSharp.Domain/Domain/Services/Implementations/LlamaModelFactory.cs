﻿using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLamaSharp.Domain.Configuration;
using Microsoft.Extensions.Options;
using SerilogTimings.Extensions;
using LLamaEmbedder = LLama.LLamaEmbedder;
using LLamaModel = LLama.LLamaModel;

namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <summary>
/// Factory for creating LLama models, executors, parameters and embedders
/// </summary>
public class LlamaModelFactory : ILlamaModelFactory
{
    private readonly ILogger logger;
    private readonly LlamaModelOptions llamaModelOptions;

    /// <summary>
    /// Constructor for LLama Model Factory
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public LlamaModelFactory(IOptions<LlamaModelOptions> options, ILogger logger)
    {
        this.logger = logger;
        ArgumentNullException.ThrowIfNull(options.Value);
        llamaModelOptions = options.Value;
    }

    /// <summary>
    /// Returns the default model parameters 
    /// </summary>
    /// <returns></returns>
    public ModelParams CreateModelParams()
    {
        return llamaModelOptions;
    }

    /// <summary>
    /// Creates a default llama model 
    /// </summary>
    /// <returns></returns>
    public LLamaModel CreateModel()
    {
        var parameters = CreateModelParams();
        return CreateModel(parameters);
    }

    /// <summary>
    /// Creates a llama model with specified parameters (ResettableLLamaModel)
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public LLamaModel CreateModel(ModelParams parameters)
    {
        using var op = logger.BeginOperation("Creating LLama Model");
        var model = new LLamaModel(parameters);    //LLamaModel   ResettableLLamaModel
        op.Complete();
        return model;
    }


    /// <summary>
    /// Creates an embedder with specified parameters 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public LLamaEmbedder CreateEmbedder(ModelParams parameters)
    {
        using var op = logger.BeginOperation("Creating Embedder");
        var model = new LLamaEmbedder(parameters);
        op.Complete();
        return model;
    }

    /// <inheritdoc />
    public (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(Action<LLamaModel>? model = default, Action<ChatSession>? chatSession = default)
       where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var lModel = CreateModel();
        model?.Invoke(lModel);
        var session = CreateChatSession<TExecutor>(lModel);
        chatSession?.Invoke(session);
        return (session, lModel);
    }


    /// <inheritdoc />
    public (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(ModelParams parameters, Action<ChatSession>? chatSession = default)
        where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var model = CreateModel(parameters);
        var session = CreateChatSession<TExecutor>(model);
        chatSession?.Invoke(session);
        return (session, model);
    }



    /// <summary>
    /// Creates a ChatSession with specified model
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public ChatSession CreateChatSession<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        using var op = logger.BeginOperation("Creating Chat Session");
        ILLamaExecutor executor = CreateStatefulExecutor<TExecutor>(model);
        var chatSession = new ChatSession(executor);
        op.Complete();
        return chatSession;
    }

    /// <summary>
    /// Creates a  StatefulExecutor using specified model
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public StatefulExecutorBase CreateStatefulExecutor<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        switch (typeof(TExecutor))
        {
            case { } type when type == typeof(InteractiveExecutor):
                return new InteractiveExecutor(model);
            case { } type when type == typeof(InstructExecutor):
                return new InstructExecutor(model);
            default:
                return (StatefulExecutorBase)Activator.CreateInstance(typeof(TExecutor), model)!;
        }
    }

    /// <summary>
    /// Creates a  StatelessExecutor using specified model
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaModel model) where TExecutor : StatelessExecutor, ILLamaExecutor
    {
        return new StatelessExecutor(model);
    }
}