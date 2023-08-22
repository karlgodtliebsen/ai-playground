using AI.Test.Support.Fixtures;

using ImageClassification.Domain.Configuration;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace ML.Net.ImageClassification.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class ImageClassificationFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMlnetImageClassification(configuration)
            ;
    }
}
