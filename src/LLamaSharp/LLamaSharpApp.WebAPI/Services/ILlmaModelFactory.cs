using LLama;
using LLama.Abstractions;
using LLama.Common;

namespace LLamaSharpApp.WebAPI.Services;

public interface ILlmaModelFactory
{
    /// <summary>
    /// Creates a ModelParams with the values obtained form IOptions<LlmaOptions>
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
    /// Creates a LLamaModel with the default ModelParams
    /// </summary>
    /// <returns></returns>
    LLamaModel CreateModel();

    /// <summary>
    /// Creates a LLamaEmbedder with the default ModelParams
    /// </summary>
    /// <returns></returns>
    LLamaEmbedder CreateEmbedder();

    /// <summary>
    /// Creates a LLamaEmbedder with the given ModelParams
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    LLamaEmbedder CreateEmbedder(ModelParams parameters);

    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor
    /// </summary>
    /// <typeparam name="TExecutor">The ILLamaExecutor type to use for the ChatSession</typeparam>
    /// <returns></returns>
    (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>() where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and LLamaModel
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="model"></param>
    /// <returns></returns>
    ChatSession CreateChatSession<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    /// <summary>
    /// Creates a ChatSession with a custom ILLamaExecutor and ModelParams
    /// </summary>
    /// <typeparam name="TExecutor"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(ModelParams parameters) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    StatefulExecutorBase CreateStatefulExecutor<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor;

    StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaModel model) where TExecutor : StatelessExecutor, ILLamaExecutor;



}
