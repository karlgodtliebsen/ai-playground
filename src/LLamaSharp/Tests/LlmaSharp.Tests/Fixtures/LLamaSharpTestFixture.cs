using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;

using LLamaSharp.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LlamaSharp.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class LLamaSharpTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        base.AddServices(services, configuration);
        services
            .AddLlamaConfiguration(configuration)
            .AddLLamaDomain(configuration)
            .AddInferenceConfiguration(configuration)
            .AddLLamaRepository(configuration)
            ;
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = configuration.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class LLamaSharpQdrantTestFixture : TestFixtureBase
{
    protected override void AddServices(IServiceCollection services, IConfigurationRoot configuration)
    {
        base.AddServices(services, configuration);
        services.AddQdrant(configuration);
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = configuration.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }

    //public TestContainerDockerLauncher Launcher { get; private set; }

    ///// <summary>
    ///// Post Build Setup of Logging and Launcher that depends on ITestOutputHelper
    ///// </summary>
    ///// <param name="output"></param>
    //public override void Setup(ITestOutputHelper output)
    //{
    //    base.Setup(output);
    //    Launcher = Factory.Services.GetRequiredService<TestContainerDockerLauncher>();
    //    Launcher.Start();
    //}

    //public void Dispose()
    //{
    //    Launcher.Stop();
    //}
}
