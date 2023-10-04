using Confluent.Kafka;

namespace WikiEditStream.Configuration;

public class KafkaConfiguration : ClientConfig
{
    public const string SectionName = "Kafka";
    public bool UserDockerTestContainer { get; set; } = false;
    public string? ApplicationId { get; set; } = default!;

    public string TopicSource { get; set; } = default!;
    public string TopicDestination { get; set; } = default!;

}
