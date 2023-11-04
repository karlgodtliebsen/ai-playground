using Confluent.Kafka;
using Confluent.Kafka.Admin;

using Kernel.Memory.NewsFeed.Domain.Configuration;

using Microsoft.Extensions.Options;

namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public sealed class KafkaAdminClient
{
    private readonly ILogger logger;
    private readonly KafkaConfiguration config;

    public KafkaAdminClient(IOptions<KafkaConfiguration> options, ILogger logger)
    {
        this.logger = logger;
        config = options.Value;
    }

    public async Task CreateTopic(CancellationToken cancellationToken)
    {

        logger.Information("{name} starting", nameof(CreateTopic));
        // Configure the admin client based on the provided configuration. 
        var adminConfig = new AdminClientConfig(config);

        // Build an admin client that uses the provided configuration.
        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        try
        {
            // Create a topic with the specified name, three partitions, and a single replica.
            //await adminClient.CreateTopicsAsync(new TopicSpecification[]
            //{
            //    new TopicSpecification
            //    {
            //        Name = config.Topic,
            //        NumPartitions = 1, //is hardcoded in this playground project. should be configures
            //        ReplicationFactor = 1
            //    }
            //});
            logger.Information("Created topic {topicName}", config.Topic);
            //await Task.Delay(1000, cancellationToken);
        }
        catch (CreateTopicsException ex)
        {
            logger.Error(ex, "An error occurred creating topic {topicName}", config.Topic);
            throw;
        }
    }
}
