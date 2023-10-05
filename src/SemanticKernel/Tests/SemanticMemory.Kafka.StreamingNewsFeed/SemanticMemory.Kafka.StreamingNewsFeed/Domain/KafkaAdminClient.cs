using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

public class KafkaAdminClient
{
    private readonly KafkaConfiguration config;

    public KafkaAdminClient(IOptions<KafkaConfiguration> options)
    {
        config = options.Value;
    }

    public async Task CreateTopic(string topicName)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        Console.WriteLine($"{nameof(CreateTopic)} starting");
        // Configure the admin client based on the provided configuration. 
        var adminConfig = new AdminClientConfig(config);

        // Build an admin client that uses the provided configuration.
        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        try
        {
            // Create a topic with the specified name, three partitions, and a single replica.
            await adminClient.CreateTopicsAsync(new TopicSpecification[]
            {
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });
            Console.WriteLine($"Created topic {topicName}");
        }
        catch (CreateTopicsException ex)
        {
            Console.WriteLine($"An error occurred creating topic {topicName}: {ex.Message}");
        }
    }

    public async Task DeleteTopic(string topicName)
    {
        Console.WriteLine($"{nameof(DeleteTopic)} starting");

        // Configure the admin client based on the provided configuration. 
        var adminConfig = new AdminClientConfig(config);

        // Build an admin client that uses the provided configuration.
        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        try
        {
            // Delete the topic with the specified name.
            await adminClient.DeleteTopicsAsync(new string[] { topicName });
            Console.WriteLine($"Deleted topic {topicName}");
        }
        catch (DeleteTopicsException ex)
        {
            Console.WriteLine($"An error occurred deleting topic {topicName}: {ex.Message}");
        }
    }

    //public async Task ListTopics()
    //{
    //    Console.WriteLine($"{nameof(ListTopics)} starting");

    //    // Configure the admin client based on the provided configuration. 
    //    var adminConfig = new AdminClientConfig(config);

    //    // Build an admin client that uses the provided configuration.
    //    using var adminClient = new AdminClientBuilder(adminConfig).Build();

    //    try
    //    {
    //        // Get the list of topics.
    //        var topics = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
    //        Console.WriteLine($"Topics: {string.Join(", ", topics.Topics.Select(t => t.Topic))}");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"An error occurred finding topic list: {ex.Message}");
    //    }
    //}
}