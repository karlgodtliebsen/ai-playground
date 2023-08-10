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
    private readonly ModelFactoryExtended factory;
    private readonly ILogger logger;

    private readonly TensorFlowOptions tensorFlowOptions;
    private readonly ExtendedTaskOptions taskOptions;


    public TensorFlowTransferLearningInception(ModelFactoryExtended factory,
        IOptions<TensorFlowOptions> tensorFlowOptions,
        IOptions<ExtendedTaskOptions> taskOptions,
        ILogger logger)
    {
        this.tensorFlowOptions = tensorFlowOptions.Value;
        this.taskOptions = taskOptions.Value;
        this.factory = factory;
        this.logger = logger;
    }


    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {
        var imageSetFolderPath = PathUtils.GetPath(tensorFlowOptions.TrainImagesFilePath, imageSetPath);
        var outputFolderPath = PathUtils.GetPath(tensorFlowOptions.OutputFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(tensorFlowOptions.InputFilePath, imageSetPath);

        var task = factory.AddImageClassificationTask<TransferLearningExtended>(
            (opt) =>
            {
                opt.DataDir = imageSetFolderPath;
                opt.InputFolderPath = inputFolderPath;
                opt.TaskPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath);
                opt.ModelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.ModelName);
                opt.LabelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.LabelFile);
                opt.Mapper = mapper;
            }
        );
        task.Train(new TrainingOptions
        {
            TrainingSteps = 100
        });
        return "ok";
    }
}
