using System.Diagnostics;
using System.Text;

using Microsoft.ML;

using ML.Net.ImageClassification.Tests.Domain;

using Serilog;

namespace ML.Net.ImageClassification.Tests;

public class ModelHelper
{

    public static void EvaluateModel(MLContext mlContext, IDataView testDataset, ITransformer trainedModel)
    {
        Log.Logger.Information("Making predictions in bulk for evaluating model's quality...");

        // Measuring time
        var watch = Stopwatch.StartNew();
        var predictionsDataView = trainedModel.Transform(testDataset);
        var metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName: "LabelAsKey", predictedLabelColumnName: "PredictedLabel");
        ReportGenerator.PrintMultiClassClassificationMetrics("TensorFlow DNN Transfer Learning", metrics);

        watch.Stop();
        var elapsed2Ms = watch.ElapsedMilliseconds;

        Log.Logger.Information("Predicting and Evaluation took: {elapsed} seconds", elapsed2Ms / 1000);
    }

    public static void TrySinglePrediction(string imagesFolderPathForPredictions, MLContext mlContext, ITransformer trainedModel)
    {
        // Create prediction function to try one prediction
        var predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(trainedModel);

        var testImages = FileUtils.LoadInMemoryImagesFromDirectory(imagesFolderPathForPredictions, false);

        var imageToPredict = testImages.First();
        var prediction = predictionEngine.Predict(imageToPredict);

        string s = "coming up";//string.Join(",",prediction.Score.Select(r=>r.ToString()));

        Log.Logger.Information("Image Filename : [{fileName}] Scores : [{scores}], Predicted Label : {predictedLabel}", imageToPredict.ImageFileName, s, prediction.PredictedLabel);

    }


    public static void WriteImagePrediction(string imagePath, string label, string predictedLabel, float probability)
    {
        var sb = new StringBuilder();

        sb.Append("Image File: ");
        sb.Append($"{Path.GetFileName(imagePath)}");
        sb.Append(" original labeled as ");
        sb.Append(label);
        sb.Append(" predicted as ");
        sb.Append(predictedLabel);
        sb.Append(" with score ");
        sb.Append(probability.ToString());

        Log.Logger.Information(sb.ToString());
    }

}
