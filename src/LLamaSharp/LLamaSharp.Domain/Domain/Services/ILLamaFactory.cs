using LLama;
using LLama.Abstractions;
using LLama.Common;

namespace LLamaSharp.Domain.Domain.Services;

/// <summary>
/// Model Factory for creating LLama models, executors, parameters and embedders
/// </summary>
public interface ILLamaFactory
{
    /// <summary>
    /// Creates a ModelParams with the values obtained form IOptions&lt;LlmaOptions&gt;
    /// </summary>
    /// <returns></returns>
    ModelParams CreateModelParams();

    /// <summary>
    /// Creates a LLamaContext with the given ModelParams
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    LLamaContext CreateContext(ModelParams parameters);

    /// <summary>
    /// Creates a LLamaContext with default ModelParams
    /// </summary>
    /// <returns></returns>
    LLamaContext CreateContext();

    /// <summary>
    /// Creates a LLamaEmbedder with the given ModelParams
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    LLamaEmbedder CreateEmbedder(ModelParams parameters);


    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and LLamaContext
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    ChatSession CreateChatSession<TExecutor>(LLamaContext model) where TExecutor : StatefulExecutorBase, ILLamaExecutor;


    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and specified ModelParams, and the ability to extend the chat session 
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="chatSession"></param>
    /// <returns></returns>
    (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(ModelParams parameters, Action<ChatSession>? chatSession = default) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and default ModelParams, and the ability to extend the chat session 
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="chatSession"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    (ChatSession chatSession, LLamaContext model) CreateChatSession<TExecutor>(Action<LLamaContext>? model = default, Action<ChatSession>? chatSession = default) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a stateful ILLamaExecutor using the specified LLamaContext
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    StatefulExecutorBase CreateStatefulExecutor<TExecutor>(LLamaContext model) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a stateless  ILLamaExecutor using the specified LLamaContext
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="TExecutor"></typeparam>
    /// <returns></returns>
    StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaContext model) where TExecutor : StatelessExecutor, ILLamaExecutor;

    InteractiveExecutor CreateInteractiveExecutor(LLamaContext model);

    InstructExecutor CreateInstructExecutor(LLamaContext model);
}
