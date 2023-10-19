using AI.Test.Support.Fixtures;

using FluentAssertions;

using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.Predictors;
using ImageClassification.Domain.Trainers;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Serilog;

using TensorFlow.Net.ImageClassification.Tests.Fixtures;

using Xunit.Abstractions;

namespace TensorFlow.Net.ImageClassification.Tests;

//Based on: https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/TransferLearningWithInceptionV3.cs

[Collection("TensorFlow Image Classification Collection")]
public class TestOfTensorFlowTransferLearningWithInception : TestFixtureBase
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly TensorFlowOptions options;

    public TestOfTensorFlowTransferLearningWithInception(TensorFlowImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBase>(output).Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        options = services.GetRequiredService<IOptions<TensorFlowOptions>>().Value;
    }


    [Theory]
    // [InlineData("meat")]
    //[InlineData("food")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    //[InlineData("fashionproducts", 0, "1-4", "styles.csv")]

    [InlineData("flowers")]
    //[InlineData("animals-10")]
    //[InlineData("animals-90")]
    //[InlineData("catsdogs")]
    //[InlineData("butterfly", 0, 1, "Training_set.csv")]
    //[InlineData("TensorFlowTransferLearning", 0, 1, "tags.tsv")]
    public void TestOfTrainingForTransferLearningWithInception(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CreateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
        }
        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = services.GetRequiredService<ITensorFlowTransferLearningInception>();
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
        ITester tester = services.GetRequiredService<ITester>();
        var accuracy = tester.TestModel(dataSet, mapper);
        accuracy.Should().BeGreaterThan(0.75f);
        var result = $"Result: ({accuracy} > {0.75f})";
        logger.Information("Training [{set}] model returned {result}", dataSet, result);
    }


    [Theory]
    [InlineData("flowers")]
    public void TestOfModelPredictionForTransferLearningWithInception(string imageSetPath)
    {
        IPredictor predictor = services.GetRequiredService<IPredictor>();
        IImageLoader loader = services.GetRequiredService<IImageLoader>();
        var options = services.GetRequiredService<IOptions<TensorFlowOptions>>().Value;

        var imageSetForPredictions = PathUtils.GetPath(options.TestImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        var images = loader.LoadInMemoryImagesFromDirectory(inputFolderPath, imageSetForPredictions);

        InMemoryImageData image = images.First();
        logger.Information("Running prediction on [{set}] {imageFile} ", imageSetPath, image.ImageFileName);
        var result = predictor.PredictImage(image, imageSetPath);
        logger.Information("Predicting result is: {result}", result);
    }

}



