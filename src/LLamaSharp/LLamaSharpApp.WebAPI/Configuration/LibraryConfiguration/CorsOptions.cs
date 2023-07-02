namespace LLamaSharpApp.WebAPI.Configuration.LibraryConfiguration;

/// <summary>
/// Cors Options
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public string SectionName { get; set; } = "Cors";

    //TODO needs fixing - work in progress

    /// <summary>
    /// Origins
    /// </summary>
    public string Origins { get; set; } = "https://localhost:4200";

    //TODO needs fixing - work in progress

    /// <summary>
    /// CORS Policy
    /// </summary>
    public string Policy { get; set; } = "AllowMyOrigins";

}
