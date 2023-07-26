using AI.Test.Support;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using ML.Net.ImageClassification.Tests.Domain;
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

    //TODO: need a strategy for handling different formats of CSV files
    //birds: birds.csv: 2.0,train/ABYSSINIAN GROUND HORNBILL/141.jpg,ABYSSINIAN GROUND HORNBILL,train,BUCORVUS ABYSSINICUS
    //butterfly: Training_set.csv: Image_1.jpg,SOUTHERN DOGFACE
    //butterfly: Testing_set.csv: Image_1.jpg
    //cars: MATLAB 5.0 MAT-file, Platform: GLNXA64, Created on: Sat Feb 28 19:34:55 2015 
    [Theory]
    //[InlineData("flowers")]
    //[InlineData("meat")]
    [InlineData("butterfly", 0, 1, "Training_set.csv")]
    ////[InlineData("food")]
    ////[InlineData("animals")]
    ////[InlineData("cars")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    public void TrainImageClassificationAndPersistModel(string dataSet, int imageIndex = -1, int labelIndex = -1, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = new ImageLabelMapper(imageIndex!, labelIndex!, fileName);
        }
        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITrainer>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }


    //[Theory]
    //[InlineData("flowers")]
    //[InlineData("meat")]
    //[InlineData("butterfly")]
    //[InlineData("food")]
    //[InlineData("animals")]
    //[InlineData("cars")]
    //[InlineData("birds")]
    public void UsePersistedImageClassificationModelToPredictImages(string dataSet)
    {
        logger.Information("Verifying [{set}]", dataSet);
        IPredictor predictor = fixture.Factory.Services.GetRequiredService<IPredictor>();
        predictor.PredictImages(dataSet);
    }
}
