using AI.Test.Support;

using FluentAssertions;

using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.Predictors;
using ImageClassification.Domain.Trainers;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;

namespace ML.Net.ImageClassification.Tests;

//Based on: https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/TransferLearningWithInceptionV3.cs

[Collection("TensorFlow Image Classification Collection")]
public class TestOfTensorFlowTransferLearningWithInception : TestFixtureBase
{
    private readonly TensorFlowImageClassificationFixture fixture;
    private readonly ILogger logger;


    public TestOfTensorFlowTransferLearningWithInception(TensorFlowImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }


    [Theory]
    [InlineData("meat")]
    //[InlineData("food")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    //[InlineData("fashionproducts", 0, "1-4", "styles.csv")]

    //[InlineData("flowers")]
    //[InlineData("animals-10")]
    //[InlineData("animals-90")]
    //[InlineData("catsdogs")]
    //[InlineData("butterfly", 0, 1, "Training_set.csv")]
    //[InlineData("cars", 5, 6, "cardatasettrain.csv")]
    //[InlineData("TensorFlowTransferLearning", 0, 1, "tags.tsv")]
    public void TestOfTrainingForTransferLearningWithInception(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CreateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
        }
        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITensorFlowTransferLearningInception>();
        var result = trainer.TrainModel(dataSet, mapper);
        result.Should().NotBeNullOrEmpty();
        logger.Information("Training [{set}] model returned {result}", dataSet, result);
    }

    [Theory]
    [InlineData("flowers")]
    public void TestOfModelTestingForTransferLearningWithInception(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CreateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
        }

        logger.Information("Training [{set}]", dataSet);
        ITester tester = fixture.Factory.Services.GetRequiredService<ITester>();
        var accuracy = tester.TestModel(dataSet, mapper);
        accuracy.Should().BeGreaterThan(0.75f);
        var result = $"Result: ({accuracy} > {0.75f})";
        logger.Information("Training [{set}] model returned {result}", dataSet, result);
    }


    [Theory]
    [InlineData("flowers")]
    public void TestOfModelPredictionForTransferLearningWithInception(string imageSetPath)
    {
        IPredictor predictor = fixture.Factory.Services.GetRequiredService<IPredictor>();
        IImageLoader loader = fixture.Factory.Services.GetRequiredService<IImageLoader>();
        var options = fixture.Factory.Services.GetRequiredService<IOptions<TensorFlowOptions>>().Value;

        var imageSetForPredictions = PathUtils.GetPath(options.TestImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        var images = loader.LoadInMemoryImagesFromDirectory(inputFolderPath, imageSetForPredictions);

        InMemoryImage image = images.First();

        logger.Information("Prediction on [{set}]", imageSetPath);

        predictor.PredictImage(image, imageSetPath);
        logger.Information("Predicting [{set}] model", imageSetPath);
    }

}



