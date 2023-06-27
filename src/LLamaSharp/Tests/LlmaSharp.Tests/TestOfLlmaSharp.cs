using Microsoft.Extensions.DependencyInjection;

using OpenAI.Tests.Utils;

using Xunit.Abstractions;

namespace OpenAI.Tests;


public class TestOfLlmaSharpClients
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly HostApplicationFactory factory;
    private readonly string path;
    //private readonly IModelRequestFactory requestFactory;

    public TestOfLlmaSharpClients(ITestOutputHelper output)
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
        //requestFactory = factory.Services.GetRequiredService<IModelRequestFactory>();
    }


    [Fact]
    public async Task Verify_WIP_()
    {

    }

}
