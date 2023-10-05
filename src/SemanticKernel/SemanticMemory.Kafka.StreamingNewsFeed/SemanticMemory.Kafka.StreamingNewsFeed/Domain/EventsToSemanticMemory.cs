using System.Text;

using Microsoft.SemanticMemory;

namespace SemanticMemory.Kafka.StreamingNewsFeed.Domain;

public interface IEventsToSemanticMemory
{
    Task AddMessage(string key, string message, CancellationToken cancellationToken);
}

public class EventsToSemanticMemory : IEventsToSemanticMemory
{
    private readonly ILogger logger;
    private readonly ISemanticMemoryClient memory;

    public EventsToSemanticMemory(ISemanticMemoryClient memory, ILogger logger)
    {
        this.memory = memory;
    }

    private static string Base64Encode(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public async Task AddMessage(string key, string message, CancellationToken cancellationToken)
    {
        var tags = new TagCollection
        {
            {
                "uri", key
            }
        };
        //steps: steps
        key = Base64Encode(key);

        var result = await
        memory
            .ImportTextAsync(message, key, index: "index001", tags: tags, cancellationToken: cancellationToken);


        //var answer = await memory.AskAsync(NasaQuestion, index: "index001");
        //logger.Information(answer.Result + "/n");

        //foreach (var x in answer.RelevantSources)
        //{
        //    logger.Information($"  * {x.SourceName} -- {x.Partitions.First().LastUpdate:D}");
        //}
    }


}
