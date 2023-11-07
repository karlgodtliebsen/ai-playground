namespace SemanticKernel.Domain.Configuration;

public class AzureOpenAIOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "AzureOpenAI";

    public string ApiKey { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public string ChatDeploymentName { get; set; } = null!;
    public string DeploymentName { get; set; }

}
