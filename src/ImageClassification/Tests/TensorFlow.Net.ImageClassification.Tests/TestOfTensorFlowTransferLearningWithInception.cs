using AI.Test.Support;

using FluentAssertions;

using ImageClassification.Domain.Models;
using ImageClassification.Domain.Trainers;

using Microsoft.Extensions.DependencyInjection;

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
    [InlineData("flowers")]
    public void TrainOfTransferLearningWithInception(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CrateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
            // mapper.LabelFileName = "imagenet_comp_graph_label_strings.txt";
        }

        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITensorFlowTransferLearningInception>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }
}



