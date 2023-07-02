using LLama.Common;

namespace LLamaSharpApp.WebAPI.Configuration;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public class LlmaModelOptions : ModelParams
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlmaModelOptions";

    /// <summary>
    /// Constructor for LlmaModelOptions
    /// </summary>
    public LlmaModelOptions() : this("./LlmaModels")
    {
    }

    /// <summary>
    /// Constructor for LlmaModelOptions
    /// </summary>
    /// <param name="modelPath"></param>
    public LlmaModelOptions(string modelPath) : base(modelPath)
    {
    }
}
