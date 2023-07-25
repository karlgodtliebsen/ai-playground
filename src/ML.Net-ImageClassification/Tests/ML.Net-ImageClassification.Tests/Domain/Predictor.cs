using Microsoft.Extensions.Options;
using Microsoft.ML;

using ML.Net.ImageClassification.Tests.Configuration;

using Serilog;

namespace ML.Net.ImageClassification.Tests.Domain;

public interface IPredictor
{
    void PredictImages(string imageSetPath);
}

public class Predictor : IPredictor
{
    private readonly ILogger logger;
    private readonly ImageClassificationOptions options;
    public Predictor(IOptions<ImageClassificationOptions> options, ILogger logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public void PredictImages(string imageSetPath)
    {
        var mlNetModelFilePath = Path.Combine(Path.GetFullPath(options.OutputFilePath.Replace("{imageSetPath}", imageSetPath)), "ImageClassifierModel.zip");
        var imageSetForPredictions = Path.Combine(Path.GetFullPath(options.InputFilePath.Replace("{imageSetPath}", imageSetPath)), "test-images");

        if (!File.Exists(mlNetModelFilePath))
        {
            logger.Information("Could not find Model: {mlNetModelFilePath}", mlNetModelFilePath);
            return;
        }

        MLContext mlContext = new MLContext(seed: 1);
        ITransformer loadedModel = mlContext.Model.Load(mlNetModelFilePath, out var modelInputSchema);

        var predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(loadedModel);

        var imagesToPredict = FileUtils.LoadInMemoryImagesFromDirectory(imageSetForPredictions, false);
        int hitCount = 0;
        int missCount = 0;
        foreach (var imageToPredict in imagesToPredict)
        {
            var prediction = predictionEngine.Predict(imageToPredict);
            var isHit = imageToPredict.FullName.Contains(prediction.PredictedLabel);
            if (isHit) hitCount++; else missCount++;

            logger.Information("\nImage Filename : [{fileName}] \nScores : [{scores}]\nPredicted Label : {predictedLabel}\nHit: {hit}", imageToPredict.FullName, prediction.Scores, prediction.PredictedLabel, isHit);
        }
        logger.Information("Hit [{fileName}] Miss: [{scores}]", hitCount, missCount);
    }
}
