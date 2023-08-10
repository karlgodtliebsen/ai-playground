﻿using LLama;
using LLama.Abstractions;
using LLama.Common;

namespace LLamaSharp.Domain.Domain.Services;

/// <summary>
/// Model Factory for creating LLama models, executors, parameters and embedders
/// </summary>
public interface ILlamaModelFactory
{
    /// <summary>
    /// Creates a ModelParams with the values obtained form IOptions&lt;LlmaOptions&gt;
    /// </summary>
    /// <returns></returns>
    ModelParams CreateModelParams();

    /// <summary>
    /// Creates a LLamaModel with the given ModelParams
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    LLamaModel CreateModel(ModelParams parameters);

    /// <summary>
    /// Creates a LLamaModel with default ModelParams
    /// </summary>
    /// <returns></returns>
    LLamaModel CreateModel();

    /// <summary>
    /// Creates a LLamaEmbedder with the given ModelParams
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    LLamaEmbedder CreateEmbedder(ModelParams parameters);


    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and LLamaModel
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    ChatSession CreateChatSession<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor;


    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and specified ModelParams, and the ability to extend the chat session 
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="chatSession"></param>
    /// <returns></returns>
    (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(ModelParams parameters, Action<ChatSession>? chatSession = default) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and default ModelParams, and the ability to extend the chat session 
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="chatSession"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(Action<LLamaModel>? model = default, Action<ChatSession>? chatSession = default) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a stateful ILLamaExecutor using the specified LLamaModel
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    StatefulExecutorBase CreateStatefulExecutor<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a stateless  ILLamaExecutor using the specified LLamaModel
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaModel model) where TExecutor : StatelessExecutor, ILLamaExecutor;

}