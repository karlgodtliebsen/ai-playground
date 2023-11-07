namespace Azure.OpenAI.Tests.Configuration;

public class AzureOpenAIConfiguration
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "AzureOpenAI";

    public string ApiKey { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string ChatDeploymentName { get; set; } = null!;

}
