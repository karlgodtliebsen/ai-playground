using LLama;
using LLama.Abstractions;
using LLama.Common;

using LLamaSharpApp.WebAPI.Configuration;
using LLamaSharpApp.WebAPI.Domain.Services;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Domain.Services.Implementations;

/// <summary>
/// Factory for creating LLama models, executors, parameters and embedders
/// </summary>
public class LlmaModelFactory : ILlmaModelFactory
{
    private readonly LlmaModelOptions llmaModelOptions;

    /// <summary>
    /// Constructor for LLama Model Factory
    /// </summary>
    /// <param name="options"></param>
    public LlmaModelFactory(IOptions<LlmaModelOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value);
        llmaModelOptions = options.Value;
    }

    /// <summary>
    /// Returns the default model parameters 
    /// </summary>
    /// <returns></returns>
    public ModelParams CreateModelParams()
    {
        return llmaModelOptions;
    }
    //default value points at "models/lamma-7B/ggml-model.bin",. Since this is located in 'models' folder that is often dedicated to code models, we override it here

    /// <summary>
    /// Creates a default llama model 
    /// </summary>
    /// <returns></returns>
    public LLamaModel CreateModel()
    {
        var parameters = CreateModelParams();
        return new LLamaModel(parameters);
    }
    /// <summary>
    /// Creates a llama model with specified parameters
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public LLamaModel CreateModel(ModelParams parameters)
    {
        return new LLamaModel(parameters);
    }

    /// <summary>
    /// Creates an embedder with specified parameters 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public LLamaEmbedder CreateEmbedder(ModelParams parameters)
    {
        return new LLamaEmbedder(parameters);
    }

    /// <summary>
    /// Creates a default ChatSession
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>() where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var model = CreateModel();
        var chatSession = CreateChatSession<TExecutor>(model);
        return (chatSession, model);
    }

    /// <summary>
    /// Creates a ChatSession with specified model parameters
    /// </summary>
    /// <param name="parameters"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(ModelParams parameters) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var model = CreateModel(parameters);
        var chatSession = CreateChatSession<TExecutor>(model);
        return (chatSession, model);
    }

    /// <summary>
    /// Creates a ChatSession with specified model
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public ChatSession CreateChatSession<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        ILLamaExecutor executor = CreateStatefulExecutor<TExecutor>(model);
        var chatSession = new ChatSession(executor);
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
