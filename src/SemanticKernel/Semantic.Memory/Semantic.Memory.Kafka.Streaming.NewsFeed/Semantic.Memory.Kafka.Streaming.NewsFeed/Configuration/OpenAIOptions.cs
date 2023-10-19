namespace SemanticMemory.Kafka.StreamingNewsFeed.Configuration;

public class OpenAIOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public const string ConfigSectionName = "OpenAI";

    public string OrganisationKey { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string Endpoint { get; set; } = "https://api.openai.com/v1/"!;

    public Uri GetBaseAddress()
    {
        return new Uri(Endpoint);
    }
}
