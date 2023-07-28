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
    [InlineData("TensorFlowTransferLearning", 0, 1, "tags.tsv")]
    public void TrainImageClassificationAndPersistModel(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CrateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
            mapper.LabelFileName = "imagenet_comp_graph_label_strings.txt";
        }

        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITensorFlowTrainer>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }
}



