using AI.Library.Utils;

using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.TransferLearning;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using SciSharp.Models;

using Serilog;

using SerilogTimings.Extensions;

using Tensorflow;

namespace ImageClassification.Domain.Predictors;

//https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/image-classification
//TODO: use the predict pool
//For improved performance and thread safety in production environments,
//use the PredictionEnginePool service, which creates an ObjectPool of
//PredictionEngine objects for use throughout your application.
//See this guide on how to use PredictionEnginePool in an ASP.NET Core Web API.
public sealed class TensorFlowTransferLearningInceptionPredictor : IPredictor
{
    private readonly ModelFactoryExtended factory;
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;
    private readonly TensorFlowOptions tensorFlowOptions;
    private readonly ExtendedTaskOptions taskOptions;


    public TensorFlowTransferLearningInceptionPredictor(ModelFactoryExtended factory,
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
    public string PredictImage(InMemoryImage image, string imageSetPath)
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
        var task = factory.AddImageClassificationTask<TransferLearningExtended>((opt) =>
        {
            opt.InputFolderPath = options.InputFolderPath;
            opt.DataDir = options.DataDir;
            opt.TaskPath = options.TaskPath;
            opt.ModelPath = options.ModelPath;
            opt.LabelPath = options.LabelPath;
        });

        //having tried to get TensorFlow to read from a byte array, I gave up and wrote the image to a temp file
        var fileName = Path.GetRandomFileName();
        Tensor? input;
        try
        {
            File.WriteAllBytes(fileName, image.Image);
            input = ImageUtil.ReadImageFromFile(fileName);
        }
        finally
        {
            File.Delete(fileName);
        }

        using var op = logger.BeginOperation("Measure of Image Prediction using {imageSetPath}...", imageSetPath);
        var result = task.Predict(input);
        op.Complete();

        logger.Information("Prediction on [{set}] Result: {result}", imageSetPath, result);
        return result.ToJson();
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
        var task = factory.AddImageClassificationTask<TransferLearningExtended>((opt) =>
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


