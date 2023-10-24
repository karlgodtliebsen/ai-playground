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
public class LLamaFactory : ILLamaFactory
{
    private readonly ILogger logger;
    private readonly LLamaModelOptions llamaModelOptions;

    /// <summary>
    /// Constructor for LLama Model Factory
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public LLamaFactory(IOptions<LLamaModelOptions> options, ILogger logger)
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

    public LLamaWeights CreateLLamaWeights(ModelParams modelParams)
    {
        var model = LLamaWeights.LoadFromFile(modelParams);
        return model;
    }

    /// <summary>
    /// Creates a default llama model 
    /// </summary>
    /// <returns></returns>
    public LLamaContext CreateContext(Microsoft.Extensions.Logging.ILogger? logger = null)
    {
        var @params = CreateModelParams();
        return CreateContext(CreateLLamaWeights(@params), @params, logger);
    }

    /// <inheritdoc />
    public LLamaContext CreateContext(LLamaWeights model, IContextParams @params, Microsoft.Extensions.Logging.ILogger? logger = null)
    {
        using var op = this.logger.BeginOperation("Creating LLama Model");
        var ctx = new LLamaContext(model, @params, logger);    //LLamaContext   ResettableLLamaContext
        op.Complete();
        return ctx;
    }

    /// <summary>
    /// Creates an embedder with specified parameters 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="params"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public LLamaEmbedder CreateEmbedder(LLamaWeights model, IContextParams @params, Microsoft.Extensions.Logging.ILogger? logger = null)
    {
        using var op = this.logger.BeginOperation("Creating Embedder");
        var embedder = new LLamaEmbedder(model, @params, logger);
        op.Complete();
        return embedder;
    }

    public (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(ModelParams parameters, Action<ChatSession>? chatSession = default, Microsoft.Extensions.Logging.ILogger? logger = null) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var lModel = CreateContext(logger);
        var session = CreateChatSession<TExecutor>(lModel);
        return (session, lModel);
    }

    /// <inheritdoc />
    public (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(Action<LLamaContext>? model = default, Action<ChatSession>? chatSession = default, Microsoft.Extensions.Logging.ILogger? logger = null)
       where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var lModel = CreateContext(logger);
        model?.Invoke(lModel);
        var session = CreateChatSession<TExecutor>(lModel);
        chatSession?.Invoke(session);
        return (session, lModel);
    }


    /// <inheritdoc />
    public (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(LLamaWeights model, IContextParams @params, Microsoft.Extensions.Logging.ILogger? logger = null, Action<ChatSession>? chatSession = default)
        where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var ctx = CreateContext(model, @params, logger);
        var session = CreateChatSession<TExecutor>(ctx);
        chatSession?.Invoke(session);
        return (session, ctx);
    }

    /// <summary>
    /// Creates a ChatSession with specified model
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public ChatSession CreateChatSession<TExecutor>(LLamaContext model, Microsoft.Extensions.Logging.ILogger? logger = null) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        using var op = this.logger.BeginOperation("Creating Chat Session");
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

    /*LLamaContext model*/

    /// <summary>
    /// Creates a  StatelessExecutor using specified model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="params"></param>
    /// <param name="logger"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    public StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaWeights model, IContextParams @params, Microsoft.Extensions.Logging.ILogger? logger = null) where TExecutor : StatelessExecutor, ILLamaExecutor
    {
        return new StatelessExecutor(model, @params, logger);
    }
}
