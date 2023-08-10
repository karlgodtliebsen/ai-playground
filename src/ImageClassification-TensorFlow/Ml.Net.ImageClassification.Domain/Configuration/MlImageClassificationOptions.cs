namespace ImageClassification.Domain.Configuration;

public class MlImageClassificationOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "MlImageClassification";

    public string TrainImagesFilePath { get; set; } = default!;
    public string TestImagesFilePath { get; set; } = default!;

    public string OutputFilePath { get; set; } = default!;

    public string InputFilePath { get; set; } = default!;

    public string ModelName { get; set; } = default!;

}
