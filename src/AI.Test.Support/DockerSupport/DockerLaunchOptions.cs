using System.Text.Json.Serialization;

namespace AI.Test.Support.DockerSupport;

public class DockerLaunchOptions
{
    public const string SectionName = "DockerLaunch";

    [JsonPropertyName("DockerSettings")]
    public DockerLaunchOption[] DockerSettings { get; set; } = { };
}
