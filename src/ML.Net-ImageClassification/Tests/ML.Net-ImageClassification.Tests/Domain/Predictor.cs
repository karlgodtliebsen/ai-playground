using System.Diagnostics;

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

        foreach (var imageToPredict in imagesToPredict)
        {
            var watch = Stopwatch.StartNew();
            //var imageToPredict = imagesToPredict.First();        //TODO: all
            var prediction = predictionEngine.Predict(imageToPredict);
            string s = "coming up"; //string.Join(",",prediction.Score.Select(r=>r.ToString()));
            logger.Information("Image Filename : [{fileName}] Scores : [{scores}], Predicted Label : {predictedLabel}", imageToPredict.ImageFileName, s, prediction.PredictedLabel);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Information("Prediction took: {elapsed} seconds", elapsedMs / 1000);
        }
    }
}
