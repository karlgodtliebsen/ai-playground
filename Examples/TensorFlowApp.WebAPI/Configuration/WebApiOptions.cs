namespace TensorFlowApp.WebAPI.Configuration;

/// <summary>
/// Holds the settings for the LLamaSharpApp.WebAPI
/// </summary>
public class WebApiOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "WebApi";

    /// <summary>
    /// Path to the StatePersistence directory
    /// </summary>
    public string StatePersistencePath { get; set; } = "./StatePersistence";

    /// <summary>
    /// Path to the ModelStatePersistence directory
    /// </summary>
    public string ModelStatePersistencePath { get; set; } = "./ModelStatePersistence";

}
