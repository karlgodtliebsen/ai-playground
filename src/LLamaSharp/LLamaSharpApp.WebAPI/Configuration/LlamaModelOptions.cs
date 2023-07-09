using LLama.Common;

namespace LLamaSharpApp.WebAPI.Configuration;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public class LlamaModelOptions : ModelParams
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlamaModel";

    /// <summary>
    /// Constructor for LlamaModelOptions
    /// </summary>
    public LlamaModelOptions() : this("./LlamaModels")
    {
    }

    /// <summary>
    /// Constructor for LlamaModelOptions
    /// </summary>
    /// <param name="modelPath"></param>
    public LlamaModelOptions(string modelPath) : base(modelPath)
    {
    }
}
