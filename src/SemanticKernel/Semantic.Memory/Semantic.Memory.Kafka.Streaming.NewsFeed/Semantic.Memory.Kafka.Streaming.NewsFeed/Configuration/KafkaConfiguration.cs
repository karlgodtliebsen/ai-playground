using Confluent.Kafka;

namespace Semantic.Memory.Kafka.Streaming.NewsFeed.Configuration;

public class KafkaConfiguration : ClientConfig
{
    public const string SectionName = "Kafka";
    public bool UserDockerTestContainer { get; set; } = false;
    public string? ApplicationId { get; set; } = default!;
    public string? GroupId { get; set; } = default!;


    public string TopicProducer { get; set; } = default!;
    public string TopicConsumer { get; set; } = default!;

}
