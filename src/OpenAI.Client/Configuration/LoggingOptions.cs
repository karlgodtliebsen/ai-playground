namespace OpenAI.Client.Configuration;

/// <summary>
/// Options for setting Serilog up
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public static string ConfigSectionName { get; set; } = "TelemetryOptions";

    /// <summary>
    /// Configuration Instance SectionName
    /// </summary>
    public string SectionName { get; set; } = ConfigSectionName;

    /// <summary>
    /// Use Telemetry
    /// </summary>
    public bool UseTelemetry { get; set; } = true;

    /// <summary>
    /// Use Request Response Logging
    /// </summary>
    public bool UseRequestResponseLogging { get; set; } = true;
}
