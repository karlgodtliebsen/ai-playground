namespace SemanticMemory.Tests.Configuration;

public class HuggingFaceOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "HuggingFace";

    public string ApiKey { get; set; } = null!;

    public string Model { get; set; } = null!;

}
