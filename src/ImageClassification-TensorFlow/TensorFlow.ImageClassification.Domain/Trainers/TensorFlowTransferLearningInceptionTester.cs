using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.TransferLearning;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using SciSharp.Models;

using Serilog;

namespace ImageClassification.Domain.Trainers;

public sealed class TensorFlowTransferLearningInceptionTester : ITester
{
    private readonly ModelFactoryExtended factory;
    private readonly ILogger logger;

    private readonly TensorFlowOptions tensorFlowOptions;
    private readonly ExtendedTaskOptions taskOptions;


    public TensorFlowTransferLearningInceptionTester(ModelFactoryExtended factory,
        IOptions<TensorFlowOptions> tensorFlowOptions,
        IOptions<ExtendedTaskOptions> taskOptions,
        ILogger logger)
    {
        this.tensorFlowOptions = tensorFlowOptions.Value;
        this.taskOptions = taskOptions.Value;
        this.factory = factory;
        this.logger = logger;
    }

    public float TestModel(string imageSetPath, ImageLabelMapper? mapper)
    {
        var imageSetFolderPath = PathUtils.GetPath(tensorFlowOptions.TestImagesFilePath, imageSetPath);
        var outputFolderPath = PathUtils.GetPath(tensorFlowOptions.OutputFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(tensorFlowOptions.InputFilePath, imageSetPath);
        var task = factory.AddImageClassificationTask<TransferLearningExtended>((opt) =>
        {
            opt.DataDir = imageSetFolderPath;
            opt.InputFolderPath = inputFolderPath;
            opt.TaskPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath);
            opt.ModelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.ModelName);
            opt.LabelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.LabelFile);
            opt.Mapper = mapper;
        });

        var result = task.Test(new TestingOptions { });
        var accuracy = result.Accuracy;
        return accuracy;
    }
}
