namespace OpenAI.Client.Configuration;

public class AzureOpenAIOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public const string ConfigSectionName = "AzureOpenAI";

    public string ApiKey { get; set; } = null!;
}
