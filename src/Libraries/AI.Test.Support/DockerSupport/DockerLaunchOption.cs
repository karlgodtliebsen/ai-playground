namespace AI.Test.Support.DockerSupport;

// ReSharper disable once ClassNeverInstantiated.Global


//TODO: https://github.com/testcontainers/testcontainers-dotnet/blob/develop/src/Testcontainers/Configurations/Containers/ContainerConfiguration.cs#L16

/// <summary>
/// use https://github.com/testcontainers/testcontainers-dotnet/blob/develop/src/Testcontainers/Configurations/Containers/ContainerConfiguration.cs#L16
/// </summary>
public sealed class DockerLaunchOption
{
    /// <summary>
    /// Name of the image to launch
    /// </summary>
    public string ImageName { get; set; } = default!;
    /// <summary>
    /// Name of the container
    /// </summary>
    public string ContainerName { get; set; } = default!;

    /// <summary>
    /// Bind specified port of the container to this port on the host
    /// </summary>
    public int HostPort { get; set; } = 6333;
    /// <summary>
    /// Bind the specified port of the container to the host port
    /// </summary>
    public int ContainerPort { get; set; } = 6333;
    /// <summary>
    /// The port to check. Waits for the port to respond
    /// </summary>
    public ushort WaitForPort { get; set; } = 6333;

    /// <summary>
    /// Bind the specified host path to the container path
    /// </summary>
    public string? HostPath { get; set; } = default!;
    /// <summary>
    /// Bind the specified container path to the host path
    /// </summary>
    public string? ContainerPath { get; set; } = default!;

    /// <summary>
    /// Environment variables to set in the container
    /// </summary>
    [Destructurama.Attributed.LogMasked()]
    public IDictionary<string, string>? EnvironmentVars { get; set; } = new Dictionary<string, string>();

}

