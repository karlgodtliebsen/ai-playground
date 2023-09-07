using LLama;
using LLama.Abstractions;
using LLama.Common;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Options;

using SerilogTimings.Extensions;


namespace LLamaSharp.Domain.Domain.Services.Implementations;

/// <summary>
/// Factory for creating LLama models, executors, parameters and embedders
/// </summary>
public class IlLamaFactory : ILLamaFactory
{
    private readonly ILogger logger;
    private readonly LLamaModelOptions LLamaContextOptions;

    /// <summary>
    /// Constructor for LLama Model Factory
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public IlLamaFactory(IOptions<LLamaModelOptions> options, ILogger logger)
    {
        this.logger = logger;
        ArgumentNullException.ThrowIfNull(options.Value);
        LLamaContextOptions = options.Value;
    }

    /// <summary>
    /// Returns the default model parameters 
    /// </summary>
    /// <returns></returns>
    public ModelParams CreateModelParams()
    {
        return LLamaContextOptions;
    }

    /// <summary>
    /// Creates a default llama model 
    /// </summary>
    /// <returns></returns>
    public LLamaContext CreateContext()
    {
        var parameters = CreateModelParams();
        return CreateContext(parameters);
    }

    /// <summary>
    /// Creates a llama model with specified parameters (ResettableLLamaContext)
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public LLamaContext CreateContext(ModelParams parameters)
    {
        using var op = logger.BeginOperation("Creating LLama Model");
        var model = new LLamaContext(parameters);    //LLamaContext   ResettableLLamaContext
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
    public (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(Action<LLamaContext>? model = default, Action<ChatSession>? chatSession = default)
       where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var lModel = CreateContext();
        model?.Invoke(lModel);
        var session = CreateChatSession<TExecutor>(lModel);
        chatSession?.Invoke(session);
        return (session, lModel);
    }


    /// <inheritdoc />
    public (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(ModelParams parameters, Action<ChatSession>? chatSession = default)
        where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var model = CreateContext(parameters);
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
    public ChatSession CreateChatSession<TExecutor>(LLamaContext model) where TExecutor : StatefulExecutorBase, ILLamaExecutor
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
    public StatefulExecutorBase CreateStatefulExecutor<TExecutor>(LLamaContext model) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        switch (typeof(TExecutor))
        {
            case { } type when type == typeof(InteractiveExecutor):
                return CreateInteractiveExecutor(model);
            case { } type when type == typeof(InstructExecutor):
                return CreateInstructExecutor(model);
            default:
                return (StatefulExecutorBase)Activator.CreateInstance(typeof(TExecutor), model)!;
        }
    }

    public InteractiveExecutor CreateInteractiveExecutor(LLamaContext model)
    {
        return new InteractiveExecutor(model);
    }
    public InstructExecutor CreateInstructExecutor(LLamaContext model)
    {
        return new InstructExecutor(model);
    }

    /// <summary>
    /// Creates a  StatelessExecutor using specified model
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaContext model) where TExecutor : StatelessExecutor, ILLamaExecutor
    {
        return new StatelessExecutor(model);
    }
}
