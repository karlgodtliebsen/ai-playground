using AI.Test.Support;
using ImageClassification.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TensorFlow.Net.ImageClassification.Tests.Fixtures;

public class TensorFlowImageClassificationFixture : TestFixtureBase
{
    public TensorFlowOptions TensorFlowOptions { get; private set; }

    public TensorFlowImageClassificationFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services
                    .AddTensorFlowImageClassification(configuration)
                    ;
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        TensorFlowOptions = Factory.Services.GetRequiredService<IOptions<TensorFlowOptions>>().Value;
    }
}
