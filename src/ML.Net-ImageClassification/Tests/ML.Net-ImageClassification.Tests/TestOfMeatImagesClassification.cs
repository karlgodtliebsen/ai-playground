using AI.Test.Support;

using Microsoft.Extensions.DependencyInjection;

using ML.Net.ImageClassification.Tests.Domain;
using ML.Net.ImageClassification.Tests.Fixtures;

using Serilog;

using Xunit.Abstractions;

namespace ML.Net.ImageClassification.Tests;


//Based on: https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/DeepLearning_ImageClassification_Training

[Collection("Image Classification Collection")]
public class TestOfMeatImagesClassification : TestFixtureBase
{

    private readonly ImageClassificationFixture fixture;
    private readonly ILogger logger;

    public TestOfMeatImagesClassification(ImageClassificationFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        fixture.Setup(output);
        this.logger = fixture.Logger;
    }

    //"meat_archive"
    [Fact]
    public void TrainMeatImageClassificationAndPersistModel()
    {
        ITrainer trainer = fixture.Factory.Services.GetRequiredService<ITrainer>();
        trainer.TrainModel("meat_archive");
    }



    [Fact]
    public void UsePersistedMeatImageClassificationModelToPredictImages()
    {
        IPredictor predicator = fixture.Factory.Services.GetRequiredService<IPredictor>();
        predicator.PredictImages("meat_archive");
    }
}
