using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.TransferLearning;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using SciSharp.Models;

using Serilog;

namespace ImageClassification.Domain.Predictors;

//https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/image-classification
//TODO: use the predict pool
//For improved performance and thread safety in production environments,
//use the PredictionEnginePool service, which creates an ObjectPool of
//PredictionEngine objects for use throughout your application.
//See this guide on how to use PredictionEnginePool in an ASP.NET Core Web API.
public sealed class TensorFlowPredictForTransferLearningInception : IPredictor
{
    private readonly ExtendedModelFactory factory;
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;
    private readonly TensorFlowOptions tensorFlowOptions;
    private readonly ExtendedTaskOptions taskOptions;


    public TensorFlowPredictForTransferLearningInception(ExtendedModelFactory factory,
          IOptions<TensorFlowOptions> tensorFlowOptions,
          IOptions<ExtendedTaskOptions> taskOptions,
          IImageLoader imageLoader, ILogger logger)
    {
        this.tensorFlowOptions = tensorFlowOptions.Value;
        this.taskOptions = taskOptions.Value;
        this.factory = factory;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    public void PredictImages(string imageSetPath, ImageLabelMapper? mapper)
    {
        var imageSetFolderPath = PathUtils.GetPath(tensorFlowOptions.TrainImagesFilePath, imageSetPath);
        var outputFolderPath = PathUtils.GetPath(tensorFlowOptions.OutputFilePath, imageSetPath);
        //load model and use it for prediction
        // predict image
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>((opt) =>
        {
            opt.DataDir = imageSetFolderPath;
            opt.TaskPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath);
            opt.ModelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.ModelName);
            opt.LabelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.LabelFile);
        });

        //TODO: fix path and hardcoded names
        var imgPath = Path.Join(imageSetFolderPath, "daisy", "5547758_eea9edfd54_n.jpg");
        var input = ImageUtil.ReadImageFromFile(imgPath);
        var result = task.Predict(input);

        logger.Information("Prediction on [{set}] Result: {result}", imageSetPath, result);

        //return result;
        //$"Result: ({accuracy} > {0.75f})";
    }
}


