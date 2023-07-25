using System.Diagnostics;

using Microsoft.Extensions.Options;
using Microsoft.ML;
using Microsoft.ML.Transforms;

using ML.Net.ImageClassification.Tests.Configuration;

using Serilog;

namespace ML.Net.ImageClassification.Tests.Domain;

public interface ITrainer
{
    void TrainModel(string imageSetPath);
}

public class Trainer : ITrainer
{
    private readonly ILogger logger;

    private readonly ImageClassificationOptions options;
    public Trainer(IOptions<ImageClassificationOptions> options, ILogger logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public void TrainModel(string imageSetPath)
    {

        var imageSetFolderPath = Path.GetFullPath(options.ImageFilePath.Replace("{imageSetPath}", imageSetPath));
        var mlNetModelFilePath = Path.Combine(Path.GetFullPath(options.OutputFilePath.Replace("{imageSetPath}", imageSetPath)), "ImageClassifierModel.zip");

        //Based on:
        //https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training
        //https://github.com/dotnet/machinelearning-samples/blob/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training/ImageClassification.Train/Program.cs#L135
        logger.Information("Starting Training...");

        var mlContext = new MLContext(seed: 1);

        // 2. Load the initial full image-set into an IDataView and shuffle so it'll be better balanced
        logger.Information("Loading images from folder: {imageSetFolderPath}", imageSetFolderPath);

        IEnumerable<ImageData> images = FileUtils.LoadImageDataFromDirectory(folder: imageSetFolderPath, useFolderNameAsLabel: true).ToList();
        IDataView fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
        IDataView shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);


        // 3. Load Images with in-memory type within the IDataView and Transform Labels to Keys (Categorical)
        IDataView shuffledFullImagesDataset = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelAsKey", inputColumnName: "Label", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
            .Append(mlContext.Transforms.LoadRawImageBytes(
                outputColumnName: "Image",
                imageFolder: imageSetFolderPath,
                inputColumnName: "ImagePath"))
            .Fit(shuffledFullImageFilePathsDataset)
            .Transform(shuffledFullImageFilePathsDataset);

        // 4. Split the data 80:20 into train and test sets, train and evaluate.
        var trainTestData = mlContext.Data.TrainTestSplit(shuffledFullImagesDataset, testFraction: 0.2);
        IDataView trainDataView = trainTestData.TrainSet;
        IDataView testDataView = trainTestData.TestSet;

        // 5. Define the model's training pipeline using DNN default values
        var pipeline = mlContext.MulticlassClassification.Trainers
            .ImageClassification(featureColumnName: "Image", labelColumnName: "LabelAsKey", validationSet: testDataView)
            .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: "PredictedLabel", inputColumnName: "PredictedLabel"));


        // Measuring training time
        var watch = Stopwatch.StartNew();

        //Train
        ITransformer trainedModel = pipeline.Fit(trainDataView);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        logger.Information("Training with transfer learning took: {elapsed} seconds", elapsedMs / 1000);

        // 7. Get the quality metrics (accuracy, etc.)
        TestHelper.EvaluateModel(mlContext, testDataView, trainedModel);

        // 8. Save the model to assets/outputs (You get ML.NET .zip model file and TensorFlow .pb model file)
        mlContext.Model.Save(trainedModel, trainDataView.Schema, mlNetModelFilePath);
        logger.Information("Model saved to: {mlNetModelFilePath}", mlNetModelFilePath);
    }

}
