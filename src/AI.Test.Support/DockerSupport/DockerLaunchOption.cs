namespace AI.Test.Support.DockerSupport;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class DockerLaunchOption
{
    public string ImageName { get; set; } = default!;
    public int HostPort { get; set; } = 6333;
    public int ContainerPort { get; set; } = 6333;
    public ushort WaitForPort { get; set; } = 6333;

    public string? HostPath { get; set; } = default!;
    public string? ContainerPath { get; set; } = default!;

    [Destructurama.Attributed.LogMasked()]
    public IDictionary<string, string> EnvironmentVars { get; set; } = new Dictionary<string, string>();

}
