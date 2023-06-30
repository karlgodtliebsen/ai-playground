using LLama.Common;

namespace LLamaSharpApp.WebAPI.Configuration;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public class LlmaOptions : ModelParams
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlmaOptions";

    public LlmaOptions() : this("./LlmaModels")
    {
    }

    public LlmaOptions(string modelPath) : base(modelPath)
    {
    }
}
