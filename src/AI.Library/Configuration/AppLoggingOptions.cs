namespace AI.Library.Configuration;

/// <summary>
/// Options for Wiring Serilog together with OpenTelemetry up
/// </summary>
public sealed class AppLoggingOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "AppLogging";

    /// <summary>
    /// Use Telemetry
    /// </summary>
    public OpenTelemetryOptions? OpenTelemetry { get; set; } = default!;

    /// <summary>
    /// Use Request Response Logging
    /// </summary>
    public bool UseRequestResponseLogging { get; set; } = false;
}
