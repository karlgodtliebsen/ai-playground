namespace LLamaSharpApp.WebAPI.Configuration.LibraryConfiguration;

/// <summary>
/// Cors Options
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// DefaultSectionName (const)
    /// </summary>
    public const string SectionName = "Cors";


    /// <summary>
    /// Origins
    /// </summary>
    public string[] Origins { get; set; } = Array.Empty<string>();


    /// <summary>
    /// CORS Policy
    /// </summary>
    public string Policy { get; set; } = default!;

}
