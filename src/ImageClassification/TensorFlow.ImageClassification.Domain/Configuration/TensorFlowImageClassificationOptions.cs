namespace ImageClassification.Domain.Configuration;

public class TensorFlowImageClassificationOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "TensorFlowImageClassification";

    public string TrainImagesFilePath { get; set; } = default!;
    public string TestImagesFilePath { get; set; } = default!;

    public string OutputFilePath { get; set; } = default!;

    public string InputFilePath { get; set; } = default!;
    public string ModelFilePath { get; set; } = default!;
    public string ModelName { get; set; } = default!;

}
