using Destructurama.Attributed;

namespace AI.Domain.Configuration;

/// <summary>
/// Options to set up telemetry
/// Uses masked protections for logging
/// </summary>
public class TelemetryOptions
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
    /// Connection string - with LogMasked attribute
    /// </summary>
    [LogMasked]
    public string? ConnectionString { get; set; }
}
