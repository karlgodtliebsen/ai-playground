using AI.Test.Support;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Configuration;

namespace ML.Net.ImageClassification.Tests.Fixtures;

public class ImageClassificationFixture : TestFixtureBase
{
    public ImageClassificationOptions ImageClassificationOptions { get; private set; }

    public ImageClassificationFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services
                    .AddImageClassification(configuration)
                    ;
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        ImageClassificationOptions = Factory.Services.GetRequiredService<IOptions<ImageClassificationOptions>>().Value;
    }
}
