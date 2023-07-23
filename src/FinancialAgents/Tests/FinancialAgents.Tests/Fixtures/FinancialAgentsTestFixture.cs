﻿using AI.Test.Support;
using AI.Test.Support.DockerSupport;

using FinancialAgents.Tests.Configuration;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;

namespace FinancialAgents.Tests.Fixtures;

public sealed class FinancialAgentsTestFixture : TestFixtureBase, IDisposable
{
    public Serilog.ILogger Logger { get; private set; }
    public Microsoft.Extensions.Logging.ILogger MsLogger { get; set; }

    public HostApplicationFactory Factory { get; private set; }

    public OpenAIOptions OpenAIOptions { get; private set; }

    public BingOptions BingOptions { get; private set; }
    public GoogleOptions GoogleOptions { get; private set; }


    public TestContainerDockerLauncher Launcher { get; private set; }
    public Func<bool> Launch { get; set; }

    public FinancialAgentsTestFixture()
    {
        Factory = HostApplicationFactory.Build(
             environment: () => "IntegrationTests",
             serviceContext: (services, configuration) =>
             {
                 services
                     .AddConfiguration(configuration)
                     //.AddQdrantVectorStore()
                     .AddOpenAIConfiguration(configuration)
                     .AddWebApiConfiguration(configuration)
                     ;
             },
             fixedDateTime: () => DateTimeOffset.UtcNow
         );
        OpenAIOptions = Factory.Services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        BingOptions = Factory.Services.GetRequiredService<IOptions<BingOptions>>().Value;
        GoogleOptions = Factory.Services.GetRequiredService<IOptions<GoogleOptions>>().Value;
    }


    public void Dispose()
    {
    }
}
