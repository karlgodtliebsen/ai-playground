using Microsoft.Extensions.Options;
using Microsoft.ML;
using Microsoft.ML.Transforms;

using ML.Net.ImageClassification.Tests.Configuration;
using ML.Net.ImageClassification.Tests.Domain.Models;

using Serilog;

using SerilogTimings;
using SerilogTimings.Extensions;

namespace ML.Net.ImageClassification.Tests.Domain;

//Based on:
//https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training
//https://github.com/dotnet/machinelearning-samples/blob/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training/ImageClassification.Train/Program.cs#L135

public sealed class Trainer : ITrainer
{
    private readonly IImageLoader imageLoader;
    private readonly IModelEvaluator modelEvaluator;
    private readonly ILogger logger;

    private readonly ImageClassificationOptions options;
    public Trainer(IOptions<ImageClassificationOptions> options, IImageLoader imageLoader, IModelEvaluator modelEvaluator, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.modelEvaluator = modelEvaluator;
        this.logger = logger;
    }

    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {
        var mlNetModelFile = PathUtils.GetPath(options.OutputFilePath, imageSetPath, options.ModelName);
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        logger.Information("Starting Training of [{imageSetPath}]...", imageSetPath);
        using Operation op0 = logger.BeginOperation("Load Images for {imageSetPath}...", imageSetPath);
        var mlContext = new MLContext(seed: 1);
        var images = imageLoader.LoadImagesMappedToLabelCategory(imageSetFolderPath, inputFolderPath, mapper).ToList();
        op0.Complete();
        logger.Information("Number of Images in Training set {set}: {count}", imageSetPath, images.Count);

        using Operation op1 = logger.BeginOperation("Shuffling Training Set and Loading Views");
        var fullImagesDataset = mlContext.Data.LoadFromEnumerable(images);
        var shuffledFullImageFilePathsDataset = mlContext.Data.ShuffleRows(fullImagesDataset);

        var shuffledFullImagesDataset = mlContext.Transforms.Conversion
                    .MapValueToKey(outputColumnName: "LabelAsKey", inputColumnName: "Label", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                    .Append(mlContext.Transforms.LoadRawImageBytes(
                        outputColumnName: "Image",
                        imageFolder: imageSetFolderPath,
                        inputColumnName: "ImagePath"))
                    .Fit(shuffledFullImageFilePathsDataset)
                    .Transform(shuffledFullImageFilePathsDataset);

        var trainTestData = mlContext.Data.TrainTestSplit(shuffledFullImagesDataset, testFraction: 0.2);
        var trainDataView = trainTestData.TrainSet;
        var testDataView = trainTestData.TestSet;

        var pipeline = mlContext.MulticlassClassification.Trainers
            .ImageClassification(featureColumnName: "Image", labelColumnName: "LabelAsKey", validationSet: testDataView)
            .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: "PredictedLabel", inputColumnName: "PredictedLabel"));
        op1.Complete();

        logger.Information("Started training with transfer learning for {set}", imageSetPath);
        using var op2 = logger.BeginOperation("Training with transfer learning", imageSetPath);
        ITransformer trainedModel = pipeline.Fit(trainDataView);
        op2.Complete();


        logger.Information("Saving Model to: {mlNetModelFilePath}", mlNetModelFile);
        using var op3 = logger.BeginOperation("Saving Model");
        mlContext.Model.Save(trainedModel, trainDataView.Schema, mlNetModelFile);
        op3.Complete();

        using var op4 = logger.BeginOperation("Evaluate Model");
        modelEvaluator.EvaluateModel(mlContext, testDataView, trainedModel);
        op4.Complete();
        return mlNetModelFile;
    }

}
