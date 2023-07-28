using AI.Test.Support;

using FluentAssertions;

using ImageClassification.Domain.Models;
using ImageClassification.Domain.Predictors;
using ImageClassification.Domain.Trainers;

using Microsoft.Extensions.DependencyInjection;

using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;

namespace ML.Net.ImageClassification.Tests;

//Based on: https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training

[Collection("Image Classification Collection")]
public class TestOfImageClassification : TestFixtureBase
{
    private readonly ImageClassificationFixture fixture;
    private readonly ILogger logger;


    public TestOfImageClassification(ImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }

    //birds: birds.csv: 2.0,train/ABYSSINIAN GROUND HORNBILL/141.jpg,ABYSSINIAN GROUND HORNBILL,train,BUCORVUS ABYSSINICUS
    //butterfly: Training_set.csv: Image_1.jpg,SOUTHERN DOGFACE
    //butterfly: Testing_set.csv: Image_1.jpg
    //cars: MATLAB 5.0 MAT-file, Platform: GLNXA64, Created on: Sat Feb 28 19:34:55 2015 
    //fachion products: 29114,Men,Accessories,Socks,Socks,Navy Blue,Summer,2012,Casual,Puma Men Pack of 3 Socks

    [Theory]
    [InlineData("flowers")]
    [InlineData("meat")]
    [InlineData("butterfly", 0, 1, "Training_set.csv")]
    [InlineData("birds", 1, 2, "birds.csv")]
    [InlineData("food")]
    [InlineData("animals")]
    [InlineData("cars", 5, 6, "cardatasettrain.csv")]
    [InlineData("animals-90")]
    [InlineData("catsdogs")]
    [InlineData("fashionproducts", 0, "1-9", "styles.csv")]
    public void TrainImageClassificationAndPersistModel(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CrateImageToLabelMapper(imageIndex, labelIndex, fileName);
        }
        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<IMlNetTrainer>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }



    [Theory]
    [InlineData("flowers")]
    [InlineData("meat")]
    [InlineData("butterfly", 0, 0, "Testing_set.csv")]
    [InlineData("food")]
    [InlineData("animals")]
    [InlineData("cars", 5, 6, "cardatasettest.csv")]
    [InlineData("birds", 1, 2, "birds.csv")]
    [InlineData("animals-90")]
    [InlineData("catsdogs")]
    [InlineData("fashionproducts", 0, "1-9", "styles.csv")]
    public void UsePersistedImageClassificationModelToPredictImages(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CrateImageToLabelMapper(imageIndex, labelIndex, fileName);
        }
        logger.Information("Verifying [{set}]", dataSet);
        IPredictor predictor = fixture.Factory.Services.GetRequiredService<IPredictor>();
        predictor.PredictImages(dataSet, mapper);
    }
}
