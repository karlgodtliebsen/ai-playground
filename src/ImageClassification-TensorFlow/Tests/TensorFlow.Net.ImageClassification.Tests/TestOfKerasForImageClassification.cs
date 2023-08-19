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
public class TestOfKerasForImageClassification : TestFixtureBase
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly TensorFlowOptions options;

    public TestOfKerasForImageClassification(TensorFlowImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithLogging(output).Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        options = services.GetRequiredService<IOptions<TensorFlowOptions>>().Value;
    }

    [Theory]
    //[InlineData("TensorFlowTransferLearning", 0, 1, "tags.tsv")]
    [InlineData("flowers")]
    public void TrainImageClassificationAndPersistModel(string dataSet, int imageIndex = -1, object? labelIndex = null, string? fileName = null)
    {
        ImageLabelMapper? mapper = null;
        if (fileName is not null)
        {
            mapper = MapImageLabels.CreateImageToLabelMapper(imageIndex, labelIndex, fileName)!;
            mapper.LabelFileName = "imagenet_comp_graph_label_strings.txt";
        }
        logger.Information("Training [{set}]", dataSet);
        ITrainer trainer = services.GetRequiredService<IKerasTrainer>();
        var model = trainer.TrainModel(dataSet, mapper);
        model.Should().NotBeNull();
    }


}
