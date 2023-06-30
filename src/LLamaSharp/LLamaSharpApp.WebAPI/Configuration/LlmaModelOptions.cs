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

    public LlmaModelOptions() : this("./LlmaModels")
    {
    }

    public LlmaModelOptions(string modelPath) : base(modelPath)
    {
    }
}
