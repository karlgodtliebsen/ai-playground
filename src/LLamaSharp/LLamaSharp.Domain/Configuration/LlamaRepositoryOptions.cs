namespace LLamaSharp.Domain.Configuration;

/// <summary>
/// Holds the settings for the LLamaSharpApp.WebAPI
/// </summary>
public class LlamaRepositoryOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlamaRepository";

    /// <summary>
    /// Path to the StatePersistence directory
    /// </summary>
    public string StatePersistencePath { get; set; } = "./StatePersistence";

    /// <summary>
    /// Path to the ModelStatePersistence directory
    /// </summary>
    public string ModelStatePersistencePath { get; set; } = "./ModelStatePersistence";

}
