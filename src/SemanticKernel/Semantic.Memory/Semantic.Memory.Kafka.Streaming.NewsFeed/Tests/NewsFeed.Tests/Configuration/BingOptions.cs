namespace SemanticMemory.Kafka.StreamingNewsFeed.Tests.Configuration;

public class BingOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "Bing";

    public string ApiKey { get; set; } = null!;
}
