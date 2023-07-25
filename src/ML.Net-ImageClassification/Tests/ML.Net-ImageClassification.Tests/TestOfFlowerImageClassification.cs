using AI.Test.Support;

using Microsoft.Extensions.DependencyInjection;

using ML.Net.ImageClassification.Tests.Domain;
using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;


namespace ML.Net.ImageClassification.Tests;


//Based on: https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training

[Collection("Image Classification Collection")]
public class TestOfFlowerImageClassification : TestFixtureBase
{

    private readonly ImageClassificationFixture fixture;
    private readonly ILogger logger;


    public TestOfFlowerImageClassification(ImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }

    [Fact]
    public void TrainFlowerImageClassificationAndPersistModel()
    {
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITrainer>();
        trainer.TrainModel("flower_photos");
    }


    [Fact]
    public void UsePersistedFlowerImageClassificationModelToPredictImages()
    {
        IPredictor predicator = fixture.Factory.Services.GetRequiredService<IPredictor>();
        predicator.PredictImages("flower_photos");
    }
}
