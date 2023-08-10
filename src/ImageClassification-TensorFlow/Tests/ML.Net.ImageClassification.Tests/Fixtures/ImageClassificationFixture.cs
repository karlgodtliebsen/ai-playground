using AI.Test.Support;
using ImageClassification.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ML.Net.ImageClassification.Tests.Fixtures;

public class ImageClassificationFixture : TestFixtureBase
{
    public MlImageClassificationOptions MlImageClassificationOptions { get; private set; }

    public ImageClassificationFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services
                    .AddMlnetImageClassification(configuration)
                    ;
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        MlImageClassificationOptions = Factory.Services.GetRequiredService<IOptions<MlImageClassificationOptions>>().Value;
    }
}
