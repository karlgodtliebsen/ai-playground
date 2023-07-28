using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.TransferLearning;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using SciSharp.Models;

using Serilog;

namespace ImageClassification.Domain.Trainers;

//Based on:
//https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/TransferLearningWithInceptionV3.cs
//uses a modified TransferLearning that is more up-to-date with the concepts fropm the latest version of .Net and DI

public sealed class TensorFlowTransferLearningInception : ITensorFlowTransferLearningInception
{
    private readonly ExtendedModelFactory factory;
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;

    private readonly TensorFlowOptions options;
    public TensorFlowTransferLearningInception(ExtendedModelFactory factory, IOptions<TensorFlowOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.factory = factory;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    private float accuracy;

    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        //var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);
        var outputFolderPath = PathUtils.GetPath(options.OutputFilePath, imageSetPath);
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>(
            (opt) =>
            {
                opt.DataDir = imageSetFolderPath;
                opt.TaskPath = Path.Combine(outputFolderPath, options.ClassificationModelPath);
            }
        );

        task.Train(new TrainingOptions
        {
            TrainingSteps = 100
        });

        Test(imageSetFolderPath, outputFolderPath);
        Predict(imageSetFolderPath, outputFolderPath);
        return $"Result: ({accuracy} > {0.75f})";
    }

    /// <summary>
    /// Prediction
    /// labels mapping, it's from output_lables.txt
    /// 0 - daisy
    /// 1 - dandelion
    /// 2 - roses
    /// 3 - sunflowers
    /// 4 - tulips
    /// </summary>
    public void Predict(string imageSetFolderPath, string outputFolderPath)
    {
        // predict image
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>((opt) =>
        {
            opt.ModelPath = Path.Combine(outputFolderPath, options.SavedModelName);
        });

        //TODO: fix path and hardcoded names
        var imgPath = Path.Join(imageSetFolderPath, "daisy", "5547758_eea9edfd54_n.jpg");
        var input = ImageUtil.ReadImageFromFile(imgPath);
        var result = task.Predict(input);
    }

    public void Test(string imageSetFolderPath, string outputFolderPath)
    {
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>((opt) =>
        {
            opt.DataDir = imageSetFolderPath;
            opt.ModelPath = Path.Combine(outputFolderPath, options.SavedModelName);
        });

        var result = task.Test(new TestingOptions
        {
        });
        accuracy = result.Accuracy;
    }
}
