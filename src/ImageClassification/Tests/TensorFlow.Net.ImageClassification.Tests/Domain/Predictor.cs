using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Configuration;

using Serilog;

namespace ML.Net.ImageClassification.Tests.Domain;

public sealed class Predictor : IPredictor
{
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;
    private readonly TensorFlowImageClassificationOptions options;
    public Predictor(IOptions<TensorFlowImageClassificationOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    public void PredictImages(string imageSetPath, ImageLabelMapper? mapper)
    {
        throw new NotImplementedException();

        //var csvFilePath = PathUtils.GetPath(options.OutputFilePath, imageSetPath, "score.csv");
        //var mlNetModelFilePath = PathUtils.GetPath(options.OutputFilePath, imageSetPath, options.ModelName);
        //var imageSetForPredictions = PathUtils.GetPath(options.TestImagesFilePath, imageSetPath);
        //var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        //if (!File.Exists(mlNetModelFilePath))
        //{
        //    logger.Information("Could not find Model: {mlNetModelFilePath}", mlNetModelFilePath);
        //    throw new FileNotFoundException(mlNetModelFilePath);
        //}
        //var mlContext = new MLContext(seed: 1);
        //ITransformer loadedModel = mlContext.Model.Load(mlNetModelFilePath, out var modelInputSchema);
        //var predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(loadedModel);
        //var imagesToPredict = imageLoader.LoadInMemoryImagesFromDirectory(inputFolderPath, imageSetForPredictions, mapper).ToList();
        //int hitCount = 0;
        //int missCount = 0;
        //foreach (var imageToPredict in imagesToPredict)
        //{
        //    using var op = logger.BeginOperation("Predicting {imageToPredict}...", imageToPredict.ImageFileName);
        //    var prediction = predictionEngine.Predict(imageToPredict);
        //    var isHit = imageToPredict.FullName.Contains(prediction.PredictedLabel);
        //    if (isHit) hitCount++; else missCount++;

        //    op.Complete();
        //    logger.Information("\nResult:\n\tImage Filename : [{fileName}] \n\tScores : [{scores}]\n\tPredicted Label : {predictedLabel}\n\tHit: {hit}", imageToPredict.FullName, prediction.Scores, prediction.PredictedLabel, isHit);

        //    string content = $"{imageToPredict.FullName},{prediction.PredictedLabel}\n";
        //    File.AppendAllText(csvFilePath, content);

        //}
        //logger.Information("Hit [{hitCount}] Miss: {missCount}", hitCount, missCount);
    }
}
