using System.Diagnostics;
using System.Globalization;
using System.Text;

using Microsoft.ML;
using ML.Net.ImageClassification.Tests.Domain.Models;
using Serilog;

namespace ML.Net.ImageClassification.Tests.Domain;

public sealed class ModelEvaluator : IModelEvaluator
{
    private readonly ILogger logger;

    public ModelEvaluator(ILogger logger)
    {
        this.logger = logger;
    }

    public void EvaluateModel(MLContext mlContext, IDataView testDataset, ITransformer trainedModel)
    {
        logger.Information("Making predictions in bulk for evaluating model's quality...");

        // Measuring time
        var watch = Stopwatch.StartNew();
        var predictionsDataView = trainedModel.Transform(testDataset);
        var metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName: "LabelAsKey", predictedLabelColumnName: "PredictedLabel");
        ReportGenerator.PrintMultiClassClassificationMetrics("TensorFlow DNN Transfer Learning", metrics);

        watch.Stop();
        var elapsed2Ms = watch.ElapsedMilliseconds;

        logger.Information("Predicting and Evaluation took: {elapsed} seconds", elapsed2Ms / 1000);
    }

    public void TrySinglePrediction(string imagesFolderPathForPredictions, MLContext mlContext, ITransformer trainedModel, IImageLoader imageLoader)
    {
        // Create prediction function to try one prediction
        var predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(trainedModel);

        var testImages = imageLoader.LoadInMemoryImagesFromDirectory(imagesFolderPathForPredictions, false);

        var imageToPredict = testImages.First();
        var prediction = predictionEngine.Predict(imageToPredict);
        logger.Information("Image Filename : [{fileName}] Scores : [{scores}], Predicted Label : {predictedLabel}", imageToPredict.ImageFileName, prediction.Scores, prediction.PredictedLabel);
    }

    public void WriteImagePrediction(string imagePath, string label, string predictedLabel, float probability)
    {
        var sb = new StringBuilder();

        sb.Append("Image File: ");
        sb.Append($"{Path.GetFileName(imagePath)}");
        sb.Append(" original labeled as ");
        sb.Append(label);
        sb.Append(" predicted as ");
        sb.Append(predictedLabel);
        sb.Append(" with score ");
        sb.Append(probability.ToString(CultureInfo.InvariantCulture));

        logger.Information(sb.ToString());
    }

}
