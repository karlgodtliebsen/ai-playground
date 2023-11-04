using System.Text;

using Microsoft.SemanticMemory;

namespace Kernel.Memory.NewsFeed.Domain.Domain.Implementation;

public sealed class EventsToSemanticMemory : IEventsToSemanticMemory
{
    private readonly ILogger logger;
    private readonly ISemanticMemoryClient semanticMemoryClient;

    public EventsToSemanticMemory(ISemanticMemoryClient semanticMemoryClient, ILogger logger)
    {
        this.semanticMemoryClient = semanticMemoryClient;
        this.logger = logger;
    }

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public async Task AddMessage(string key, string message, CancellationToken cancellationToken)
    {
        logger.Information($"Adding message to memory: {message}");
        var tags = new TagCollection
        {
            {
                "uri", key
            }
        };
        //steps: steps
        key = Base64Encode(key);

        var result = await
        semanticMemoryClient
            .ImportTextAsync(message, key, index: "index001", tags: tags, cancellationToken: cancellationToken);

    }
}
