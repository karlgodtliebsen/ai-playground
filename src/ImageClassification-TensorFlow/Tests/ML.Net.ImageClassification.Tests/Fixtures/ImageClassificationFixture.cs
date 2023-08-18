using AI.Test.Support.Fixtures;

using ImageClassification.Domain.Configuration;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace ML.Net.ImageClassification.Tests.Fixtures;

public class ImageClassificationFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        base.AddServices(services, configuration);
        services
            .AddMlnetImageClassification(configuration)
            ;
    }
}
