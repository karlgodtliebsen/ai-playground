using Confluent.Kafka;

namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public class KafkaConfiguration : ClientConfig
{
    public const string SectionName = "Kafka";
    public bool UserDockerTestContainer { get; set; } = false;
    public string? ApplicationId { get; set; } = default!;
    public string? GroupId { get; set; } = default!;

    public string Topic { get; set; } = default!;

}
