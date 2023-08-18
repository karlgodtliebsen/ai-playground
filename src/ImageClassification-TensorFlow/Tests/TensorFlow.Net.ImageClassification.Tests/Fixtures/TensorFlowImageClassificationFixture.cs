using AI.Test.Support.Fixtures;

using ImageClassification.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TensorFlow.Net.ImageClassification.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class TensorFlowImageClassificationFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        base.AddServices(services, configuration);
        services
            .AddTensorFlowImageClassification(configuration)
            ;
    }
}
