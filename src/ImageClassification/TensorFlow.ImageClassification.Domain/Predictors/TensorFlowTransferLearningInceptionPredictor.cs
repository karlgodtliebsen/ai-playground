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
public sealed class TensorFlowTransferLearningInceptionPredictor : IPredictor
{
    private readonly ExtendedModelFactory factory;
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;
    private readonly TensorFlowOptions tensorFlowOptions;
    private readonly ExtendedTaskOptions taskOptions;


    public TensorFlowTransferLearningInceptionPredictor(ExtendedModelFactory factory,
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
        var outputFolderPath = PathUtils.GetPath(tensorFlowOptions.OutputFilePath, imageSetPath);
        var options = new ExtendedTaskOptions()
        {
            InputFolderPath = PathUtils.GetPath(tensorFlowOptions.InputFilePath, imageSetPath),
            DataDir = PathUtils.GetPath(tensorFlowOptions.TestImagesFilePath, imageSetPath),
            TaskPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath),
            ModelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.ModelName),
            LabelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.LabelFile),
        };

        //load model and use it for prediction
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>((opt) =>
        {
            opt.InputFolderPath = options.InputFolderPath;
            opt.DataDir = options.DataDir;
            opt.TaskPath = options.TaskPath;
            opt.ModelPath = options.ModelPath;
            opt.LabelPath = options.LabelPath;
            opt.Mapper = mapper;
        });

        var images = imageLoader.LoadImagesMappedToLabelCategory(options.DataDir, options.InputFolderPath!, options.Mapper).ToList();
        foreach (var imageData in images)
        {
            var input = ImageUtil.ReadImageFromFile(imageData.FullFileName());
            var result = task.Predict(input);
            logger.Information("Prediction on [{set}] {image} Result: {result}", imageSetPath, imageData.ImagePath, result);
        }
    }
}


