namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public sealed class LlamaModelRequestResponse
{

    /// <summary>Model context size (n_ctx)</summary>
    public int? ContextSize { get; set; } = 512;

    /// <summary>
    /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
    /// </summary>
    public int? GpuLayerCount { get; set; } = 20;

    /// <summary>Seed for the random number generator (seed)</summary>
    public int? Seed { get; set; } = 1686349486;

    /// <summary>Use f16 instead of f32 for memory kv (memory_f16)</summary>
    public bool? UseFp16Memory { get; set; } = true;

    /// <summary>Use mmap for faster loads (use_mmap)</summary>
    public bool? UseMemorymap { get; set; } = true;

    /// <summary>Use mlock to keep model in memory (use_mlock)</summary>
    public bool? UseMemoryLock { get; set; } = false;

    /// <summary>Compute perplexity over the prompt (perplexity)</summary>
    public bool? Perplexity { get; set; } = false;


    /// <summary>lora adapter path (lora_adapter)</summary>
    public string? LoraAdapter { get; set; } = string.Empty;

    /// <summary>base model path for the lora adapter (lora_base)</summary>
    public string? LoraBase { get; set; } = string.Empty;

    /// <summary>Number of threads (-1 = autodetect) (n_threads)</summary>
    public int? Threads { get; set; } = Math.Max(Environment.ProcessorCount / 2, 1);

    /// <summary>
    /// batch size for prompt processing (must be &gt;=32 to use BLAS) (n_batch)
    /// </summary>
    public int? BatchSize { get; set; } = 512;

    /// <summary>
    /// Whether to convert eos to newline during the inference.
    /// </summary>
    public bool? ConvertEosToNewLine { get; set; } = false;

    /// <summary>
    /// Whether to use embedding mode. (embedding) Note that if this is set to true,
    /// The LLamaModel won't produce text response anymore.
    /// </summary>
    public bool? EmbeddingMode { get; set; } = false;

    /// <summary>
    /// The Name of the Model
    /// </summary>
    public string? ModelName { get; set; } = default!;

}
