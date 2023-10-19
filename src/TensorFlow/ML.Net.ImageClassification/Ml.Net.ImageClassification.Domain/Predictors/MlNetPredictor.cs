using AI.Library.Utils;

using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;
using Microsoft.ML;

using Serilog;

using SerilogTimings.Extensions;

namespace ImageClassification.Domain.Predictors;

//https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/image-classification
//TODO: use the predict pool
//For improved performance and thread safety in production environments,
//use the PredictionEnginePool service, which creates an ObjectPool of
//PredictionEngine objects for use throughout your application.
//See this guide on how to use PredictionEnginePool in an ASP.NET Core Web API.
public sealed class MlNetPredictor : IPredictor
{
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;
    private readonly MlImageClassificationOptions options;
    public MlNetPredictor(IOptions<MlImageClassificationOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    public string PredictImage(InMemoryImage image, string imageSetPath)
    {
        var mlNetModelFilePath = PathUtils.GetPath(options.OutputFilePath, imageSetPath, options.ModelName);
        if (!File.Exists(mlNetModelFilePath))
        {
            logger.Information("Could not find Model: {mlNetModelFilePath}", mlNetModelFilePath);
            throw new FileNotFoundException(mlNetModelFilePath);
        }
        var mlContext = new MLContext(seed: 1);
        var loadedModel = mlContext.Model.Load(mlNetModelFilePath, out var modelInputSchema);
        using var op = logger.BeginOperation("Measure of Image Prediction using {imageSetPath}...", imageSetPath);
        var predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImage, ImagePrediction>(loadedModel);
        ImagePrediction? prediction = predictionEngine.Predict(image);
        op.Complete();

        logger.Information("Prediction on [{set}] Result: {result}", imageSetPath, prediction);
        return prediction.ToJson();

        //return $"{prediction.PredictedLabel} : {prediction.Scores}"; //.ToJson();
    }

    public void PredictImages(string imageSetPath, ImageLabelMapper? mapper)
    {
        var csvFilePath = PathUtils.GetPath(options.OutputFilePath, imageSetPath, "score.csv");
        var mlNetModelFilePath = PathUtils.GetPath(options.OutputFilePath, imageSetPath, options.ModelName);
        var imageSetForPredictions = PathUtils.GetPath(options.TestImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        if (!File.Exists(mlNetModelFilePath))
        {
            logger.Information("Could not find Model: {mlNetModelFilePath}", mlNetModelFilePath);
            throw new FileNotFoundException(mlNetModelFilePath);
        }
        var mlContext = new MLContext(seed: 1);
        var loadedModel = mlContext.Model.Load(mlNetModelFilePath, out var modelInputSchema);
        var predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(loadedModel);
        var imagesToPredict = imageLoader.LoadInMemoryImagesFromDirectory(inputFolderPath, imageSetForPredictions, mapper).ToList();
        logger.Information("Number of Images in Prediction set: {set} is {count}", imageSetPath, imagesToPredict.Count);

        var hitCount = 0;
        var missCount = 0;
        foreach (var imageToPredict in imagesToPredict)
        {
            using var op = logger.BeginOperation("Predicting {imageToPredict}...", imageToPredict.ImageFileName);
            var prediction = predictionEngine.Predict(imageToPredict);
            var isHit = imageToPredict.FullName.Contains(prediction.PredictedLabel);
            if (isHit) hitCount++; else missCount++;
            op.Complete();
            logger.Information("\nResult:\n\tImage Filename : [{fileName}] \n\tScores : [{scores}]\n\tPredicted Label : {predictedLabel}\n\tHit: {hit}", imageToPredict.FullName, prediction.Scores, prediction.PredictedLabel, isHit);
            var content = $"{imageToPredict.FullName},{prediction.PredictedLabel}\n";
            File.AppendAllText(csvFilePath, content);

        }
        logger.Information("Hit [{hitCount}] Miss: {missCount}", hitCount, missCount);
    }
}
