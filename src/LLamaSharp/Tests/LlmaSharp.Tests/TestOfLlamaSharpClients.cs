using Microsoft.Extensions.DependencyInjection;

using OpenAI.Tests.Utils;

using Xunit.Abstractions;

namespace OpenAI.Tests;


public class TestOfLlamaSharpClients
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly string path;

    public TestOfLlamaSharpClients(ITestOutputHelper output)
    {
        this.output = output;
        this.factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                //services.AddOpenAIConfiguration(configuration);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

    }


    [Fact]
    public async Task Verify_WIP_()
    {
        throw new NotImplementedException();
    }

}
