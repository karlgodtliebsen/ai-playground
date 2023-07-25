using AI.Test.Support;

using Microsoft.Extensions.DependencyInjection;

using ML.Net.ImageClassification.Tests.Domain;
using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;


namespace ML.Net.ImageClassification.Tests;


//Based on: https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training

[Collection("Image Classification Collection")]
public class TestOfFlowerImageClassification : TestFixtureBase
{

    private readonly ImageClassificationFixture fixture;
    private readonly ILogger logger;


    public TestOfFlowerImageClassification(ImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }

    [Fact]
    public void TrainFlowerImageClassificationAndPersistModel()
    {
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITrainer>();
        trainer.TrainModel("flower_photos");
    }



    [Fact]
    public void UsePersistedFlowerImageClassificationModelToPredictImages()
    {
        IPredictor predicator = fixture.Factory.Services.GetRequiredService<IPredictor>();
        predicator.PredictImages("flower_photos");
    }


    /*

    [Fact]
    public void TrainAndPersistFlowerClassificationModel()
    {
        //Based on:
        //https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training
        //https://github.com/dotnet/machinelearning-samples/blob/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training/ImageClassification.Train/Program.cs#L135
        logger.Information("Starting Training of Flower Classification...");

        var mlContext = new MLContext(seed: 1);

        // 2. Load the initial full image-set into an IDataView and shuffle so it'll be better balanced
        logger.Information("Loading images from folder: {imageSetFolderPath}", imageSetFolderPath);

        IEnumerable<ImageData> images = FileUtils.LoadImageDataFromDirectory(folder: imageSetFolderPath, useFolderNameAsLabel: true);
        IDataView fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
        IDataView shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);


        // 3. Load Images with in-memory type within the IDataView and Transform Labels to Keys (Categorical)
        IDataView shuffledFullImagesDataset = mlContext.Transforms.Conversion.
            MapValueToKey(outputColumnName: "LabelAsKey", inputColumnName: "Label", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
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

        // 9. Try a single prediction simulating an end-user app
        TestHelper.TrySinglePrediction(imageSetForPredictions, mlContext, trainedModel);
    }


    [Fact]
    public void UsePersistedFlowerClassificationModel()
    {
        if (!File.Exists(mlNetModelFilePath))
        {
            logger.Information("Could not find Model: {mlNetModelFilePath}", mlNetModelFilePath);
            logger.Information("Starting Training...");
            TrainAndPersistFlowerClassificationModel();
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
            string s = "coming up";//string.Join(",",prediction.Score.Select(r=>r.ToString()));
            logger.Information("Image Filename : [{fileName}] Scores : [{scores}], Predicted Label : {predictedLabel}", imageToPredict.ImageFileName, s, prediction.PredictedLabel);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            logger.Information("Prediction took: {elapsed} seconds", elapsedMs / 1000);
        }
    }
    */

}
