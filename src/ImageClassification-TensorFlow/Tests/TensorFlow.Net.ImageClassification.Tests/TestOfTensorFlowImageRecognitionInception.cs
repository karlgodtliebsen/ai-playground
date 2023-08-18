using AI.Test.Support.Fixtures;

using FluentAssertions;

using ImageClassification.Domain.Configuration;

using ImageClassification.Domain.Models;
using ImageClassification.Domain.Trainers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Serilog;

using TensorFlow.Net.ImageClassification.Tests.Fixtures;

using Xunit.Abstractions;

namespace TensorFlow.Net.ImageClassification.Tests;

//Based on: 

[Collection("TensorFlow Image Classification Collection")]
public class TestOfTensorFlowImageRecognitionInception : TestFixtureBase
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly TensorFlowOptions options;

    public TestOfTensorFlowImageRecognitionInception(TensorFlowImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.BuildFactoryWithLogging(output);
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        options = services.GetRequiredService<IOptions<TensorFlowOptions>>().Value;
    }


    [Theory]
    //[InlineData("TensorFlowTransferLearning", 0, 1, "tags.tsv")]
    [InlineData("flowers")]
    //[InlineData("meat")]
    //[InlineData("butterfly", 0, 1, "Training_set.csv")]
    //[InlineData("birds", 1, 2, "birds.csv")]
    //[InlineData("food")]
    //[InlineData("animals-10")]
    //[InlineData("cars", 5, 6, "cardatasettrain.csv")]
    //[InlineData("animals-90")]
    //[InlineData("catsdogs")]
    //[InlineData("fashionproducts", 0, "1-4", "styles.csv")]
    public void TestOfTensorFlowImageClassification(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CreateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
        }

        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = services.GetRequiredService<ITensorFlowTrainer>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }
}



