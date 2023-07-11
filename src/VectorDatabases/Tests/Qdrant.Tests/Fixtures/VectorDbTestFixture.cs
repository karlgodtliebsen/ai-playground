using AI.Test.Support;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit.Abstractions;

namespace Qdrant.Tests.Fixtures;

public sealed class VectorDbTestFixture : IDisposable
{
    public ILogger Logger { get; private set; }
    public HostApplicationFactory Factory { get; private set; }
    public QdrantOptions Options { get; private set; }

    public Func<ITestOutputHelper> GetOutput { get; set; }

    public VectorDbTestFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddQdrant(configuration);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
            //output: () => GetOutput()
        );
        Logger = Factory.Services.GetRequiredService<ILogger>();
        Options = Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value;
    }

    public void Dispose()
    {

    }
}
