using System.Text;

using Kernel.Memory.NewsFeed.Domain.Configuration;

using Microsoft.Extensions.Options;
using Microsoft.SemanticMemory;

namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public sealed class EventsToSemanticMemory : IEventsToSemanticMemory
{
    private readonly ILogger logger;
    private readonly ISemanticMemoryClient semanticMemoryClient;
    private readonly OpenSearchConfiguration openSearchConfiguration;
    private readonly string indexName;

    public EventsToSemanticMemory(ISemanticMemoryClient semanticMemoryClient, IOptions<OpenSearchConfiguration> openSearchOptions, ILogger logger)
    {
        this.semanticMemoryClient = semanticMemoryClient;
        this.openSearchConfiguration = openSearchOptions.Value;
        this.logger = logger;
        indexName = openSearchConfiguration.Indices[0];
    }

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public async Task AddMessage(string key, string message, CancellationToken cancellationToken)
    {
        logger.Information($"Adding message to memory: {message}");
        var tags = new TagCollection() { { "uri", key } };
        //steps: steps
        key = Base64Encode(key);
        try
        {
            await
            semanticMemoryClient
                .ImportTextAsync(message, key, index: indexName, tags: tags, cancellationToken: cancellationToken);

        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error adding message to memory");
        }

    }
}
