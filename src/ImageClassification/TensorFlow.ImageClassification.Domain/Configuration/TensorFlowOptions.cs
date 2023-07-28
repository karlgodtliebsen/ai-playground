namespace ImageClassification.Domain.Configuration;

public class TensorFlowOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "TensorFlow";

    public string TrainImagesFilePath { get; set; } = default!;
    public string TestImagesFilePath { get; set; } = default!;

    public string OutputFilePath { get; set; } = default!;

    public string InputFilePath { get; set; } = default!;
    public string ModelFilePath { get; set; } = default!;
    public string ModelName { get; set; } = "tensorflow_inception_graph.pb";

}
