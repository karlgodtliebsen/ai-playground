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
    ModelParams CreateParams();


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
    /// Creates a ChatSession with a custom ILLamaExecutor
    /// </summary>
    /// <typeparam name="TExecutor">The ILLamaExecutor type to use for the ChatSession</typeparam>
    /// <returns></returns>
    ChatSession CreateChatSession<TExecutor>() where TExecutor : StatefulExecutorBase, ILLamaExecutor;
}
