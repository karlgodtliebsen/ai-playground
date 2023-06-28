using LLama;
using LLama.Abstractions;
using LLama.Common;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.Extensions.Options;

namespace LLamaSharpApp.WebAPI.Services;

public class LlmaModelFactory : ILlmaModelFactory
{
    private readonly LlmaOptions options;

    public LlmaModelFactory(IOptions<LlmaOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value);
        this.options = options.Value;
    }

    public ModelParams CreateParams()
    {
        //TODO: a mapper like mapperly would be nice
        //default value points at "models/lamma-7B/ggml-model.bin",. Since this is located in 'models' folder that is often dedicated to code models, we override it here
        var llmaParams = new ModelParams(
            modelPath: options.Model!,
            contextSize: options.ContextSize!.Value,
            gpuLayerCount: options.GpuLayerCount!.Value,
            seed: options.Seed!.Value,
            useFp16Memory: options.UseFp16Memory!.Value,
            useMemorymap: options.UseMemorymap!.Value,
            useMemoryLock: options.UseMemoryLock!.Value,
            perplexity: options.Perplexity!.Value,
            loraAdapter: options.LoraAdapter,
            loraBase: options.LoraBase,
            threads: options.Threads!.Value,
            batchSize: options.BatchSize!.Value,
            convertEosToNewLine: options.ConvertEosToNewLine!.Value,
            embeddingMode: options.EmbeddingMode!.Value
        );
        return llmaParams;
    }


    public LLamaModel CreateModel()
    {
        var parameters = CreateParams();
        var model = new LLamaModel(parameters);//LlmaSharp Design: Should Use Interface for  LLamaModel
        return model;
    }

    public LLamaEmbedder CreateEmbedder()
    {
        var embedder = new LLamaEmbedder(new ModelParams(options.Model!));//LlmaSharp Design: Should Use Interface for  LLamaEmbedder
        return embedder;
    }

    //LlmaSharp Design: Should Use Interface for  LLamaEmbedder
    public ChatSession CreateChatSession<TExecutor>() where TExecutor : StatefulExecutorBase, ILLamaExecutor
    {
        var model = CreateModel();
        ILLamaExecutor executor;
        switch (typeof(TExecutor))
        {
            case { } type when type == typeof(InteractiveExecutor):
                executor = new InteractiveExecutor(model);
                break;
            case { } type when type == typeof(InstructExecutor):
                executor = new InstructExecutor(model);
                break;
            default:
                executor = (ILLamaExecutor)Activator.CreateInstance(typeof(TExecutor), model)!;
                break;
        }
        var chatSession = new ChatSession(executor);
        return chatSession;
    }

}
