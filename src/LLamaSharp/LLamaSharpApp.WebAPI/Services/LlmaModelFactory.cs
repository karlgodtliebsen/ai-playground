using LLama;
using LLama.Abstractions;
using LLama.Common;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Services;

public class LlmaModelFactory : ILlmaModelFactory
{
    private readonly LlmaOptions llmaOptions;

    public LlmaModelFactory(IOptions<LlmaOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value);
        this.llmaOptions = options.Value;
    }

    public ModelParams CreateModelParams()
    {
        return llmaOptions;
    }
    //default value points at "models/lamma-7B/ggml-model.bin",. Since this is located in 'models' folder that is often dedicated to code models, we override it here

    public LLamaModel CreateModel()
    {
        ModelParams parameters = CreateModelParams();
        return new LLamaModel(parameters);                     //LlmaSharp Design smell: Should Use Interface for  LLamaModel
    }
    public LLamaModel CreateModel(ModelParams parameters)
    {
        return new LLamaModel(parameters);                     //LlmaSharp Design smell: Should Use Interface for  LLamaModel
    }

    public LLamaEmbedder CreateEmbedder()
    {
        return new LLamaEmbedder(CreateModelParams());      //LlmaSharp Design smell: Should Use Interface for  LLamaEmbedder
    }
    public LLamaEmbedder CreateEmbedder(ModelParams parameters)
    {
        return new LLamaEmbedder(parameters);      //LlmaSharp Design smell: Should Use Interface for  LLamaEmbedder
    }

    //LlmaSharp Design smell: Should Use Interface for  LLamaEmbedder
    public (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>() where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        LLamaModel model = CreateModel();
        var chatSession = CreateChatSession<TExecutor>(model);
        return (chatSession, model);
    }

    public (ChatSession chatSession, LLamaModel model) CreateChatSession<TExecutor>(ModelParams parameters) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        LLamaModel model = CreateModel(parameters);
        var chatSession = CreateChatSession<TExecutor>(model);
        return (chatSession, model);
    }

    public ChatSession CreateChatSession<TExecutor>(LLamaModel model) where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        ILLamaExecutor executor = CreateStatefulExecutor<TExecutor>(model);
        var chatSession = new ChatSession(executor);
        return chatSession;
    }

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

    public StatelessExecutor CreateStateLessExecutor<TExecutor>(LLamaModel model) where TExecutor : StatelessExecutor, ILLamaExecutor
    {
        return new StatelessExecutor(model);
    }
}
