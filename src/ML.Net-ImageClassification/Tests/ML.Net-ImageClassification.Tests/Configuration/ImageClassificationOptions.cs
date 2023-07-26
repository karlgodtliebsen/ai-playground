namespace ML.Net.ImageClassification.Tests.Configuration;

public class ImageClassificationOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "ImageClassification";

    public string TrainImagesFilePath { get; set; } = default!;
    public string TestImagesFilePath { get; set; } = default!;

    public string OutputFilePath { get; set; } = default!;

    public string InputFilePath { get; set; } = default!;

    public string ModelName { get; set; } = default!;

}
