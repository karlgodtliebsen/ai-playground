using LLama;
using LLama.Abstractions;
using LLama.Common;

namespace LLamaSharpApp.WebAPI.Domain.Services;

/// <summary>
/// Model Factory for creating LLama models, executors, parameters and embedders
/// </summary>
public interface ILlmaModelFactory
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
    /// Creates a ChatSession with a custom ILLamaExecutor and specified ModelParams
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(ModelParams parameters) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

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
