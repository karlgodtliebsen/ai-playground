namespace OpenAI.Client.Configuration;

public class OpenAIConfiguration
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public const string SectionName = "OpenAI";

    public string OrganisationKey { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string Endpoint { get; set; } = "https://api.openai.com/v1/"!;

    public Uri GetBaseAddress()
    {
        return new Uri(Endpoint);
    }
}
