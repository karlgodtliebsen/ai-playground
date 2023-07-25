namespace ML.Net.ImageClassification.Tests.Configuration;

public class ImageClassificationOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "ImageClassification";

    public string ImageFilePath { get; set; }

    public string OutputFilePath { get; set; }
    public string InputFilePath { get; set; }
}
