using AI.Test.Support;

using FluentAssertions;

using ImageClassification.Domain.Models;
using ImageClassification.Domain.Trainers;

using Microsoft.Extensions.DependencyInjection;

using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;

namespace ML.Net.ImageClassification.Tests;

//Based on: 

[Collection("TensorFlow Image Classification Collection")]
public class TestOfTensorFlowImageRecognitionInception : TestFixtureBase
{
    private readonly TensorFlowImageClassificationFixture fixture;
    private readonly ILogger logger;


    public TestOfTensorFlowImageRecognitionInception(TensorFlowImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }


    [Theory]
    //[InlineData("TensorFlowTransferLearning", 0, 1, "tags.tsv")]
    [InlineData("flowers")]
    //[InlineData("meat")]
    //[InlineData("butterfly", 0, 1, "Training_set.csv")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    //[InlineData("food")]
    //[InlineData("animals")]
    //[InlineData("cars", 5, 6, "cardatasettrain.csv")]
    //[InlineData("animals-90")]
    //[InlineData("catsdogs")]
    //[InlineData("fashionproducts", 0, "1-9", "styles.csv")]
    public void TestOfTensorFlowImageClassification(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CreateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
        }

        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITensorFlowTrainer>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }
}



