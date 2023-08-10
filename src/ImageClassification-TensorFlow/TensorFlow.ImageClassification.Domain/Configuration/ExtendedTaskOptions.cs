using ImageClassification.Domain.Models;

using SciSharp.Models;

namespace ImageClassification.Domain.Configuration;

public class ExtendedTaskOptions : TaskOptions
{
    /// <summary>
    /// Configuration Section Name
    /// </summary>
    public const string SectionName = "ExtendedTask";

    public string TaskPath { get; set; } = "image_classification_v1";
    public string ModelName { get; set; } = "saved_model.pb";
    public string LabelFile { get; set; } = "labels.txt";
    public string RetrainLogs { get; set; } = "retrain_logs";
    public string Bottleneck { get; set; } = "bottleneck";
    public string TfModulesZipFile { get; set; } = "tfhub_modules.zip";
    public string TfModules { get; set; } = "tfhub_modules";

    public string ClassificationModelPath { get; set; } = "image_classification_v1";

    public string MetaDataPath { get; set; } = "graph";
    public string MetaDataFilename { get; set; } = "InceptionV3.meta";

    public string MetaDataUrl { get; set; } = "https://raw.githubusercontent.com/SciSharp/TensorFlow.NET/master/graph/InceptionV3.meta";

    public string CheckpointDataUrl { get; set; } = "https://github.com/SciSharp/TensorFlow.NET/raw/master/data/tfhub_modules.zip";

    public ImageLabelMapper? Mapper { get; set; } = default!;
    public string? InputFolderPath { get; set; } = default!;
}
