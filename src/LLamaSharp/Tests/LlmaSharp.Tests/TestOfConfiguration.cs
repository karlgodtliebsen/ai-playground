using AI.Library.HttpUtils.LibraryConfiguration;

using FluentAssertions;

using LlamaSharp.Tests.Fixtures;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.DomainServices;
using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Services;

using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Serilog;

using Xunit.Abstractions;

namespace LlamaSharp.Tests;

public sealed class TestOfConfiguration : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly ITestOutputHelper output;
    private readonly IntegrationTestWebApplicationFactory factory;

    public TestOfConfiguration(IntegrationTestWebApplicationFactory factory, ITestOutputHelper output)
    {
        this.output = output;
        this.factory = factory
            .WithOutputLogSupport(output)
            .Build<IntegrationTestWebApplicationFactory>();
    }

    public void Dispose()
    {
        Log.CloseAndFlush();
    }

    [Fact]
    public void EmitLogEntry()
    {
        output.WriteLine("Started log test");

        var seriLogger = factory.Services.GetService<ILogger>();
        seriLogger.Information("Hello Serilog!");

        factory.Logger.Information("Hello from Factory Serilog!");
        factory.MsLogger.LogInformation("Hello Factory MsLog!");
        //var msLogger = factory.Services.GetService<Microsoft.Extensions.Logging.ILogger>();
        //msLogger.LogInformation("Hello MsLog!");

        var loggerFactory = factory.Services.GetService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Test");
        logger.LogInformation("Hello ILogger!");
        output.WriteLine("Ended log test");
    }

    [Fact]
    public void EnsureThatServiceConfigurationIsValid()
    {
        factory.Services.GetService<ILogger>().Should().NotBeNull();

        factory.Services.GetService<ILoggerFactory>().Should().NotBeNull();
        factory.Services.GetService<Microsoft.Extensions.Logging.ILogger>().Should().NotBeNull();
        factory.Services.GetService<ILogger<object>>().Should().NotBeNull();

        factory.Services.GetService<ILLamaFactory>().Should().NotBeNull();
        factory.Services.GetService<IOptionsService>().Should().NotBeNull();
        factory.Services.GetService<ICompositeService>().Should().NotBeNull();
        factory.Services.GetService<IChatService>().Should().NotBeNull();
        factory.Services.GetService<IEmbeddingsService>().Should().NotBeNull();
        factory.Services.GetService<IInteractiveExecutorService>().Should().NotBeNull();
        factory.Services.GetService<IContextStateRepository>().Should().NotBeNull();
        factory.Services.GetService<IUsersStateRepository>().Should().NotBeNull();

        factory.Services.GetService<RequestMessagesMapper>().Should().NotBeNull();
        factory.Services.GetService<OptionsMapper>().Should().NotBeNull();


        //Scoped service registrations
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetService<IUserIdProvider>().Should().NotBeNull();

        //Options
        factory.Services.GetService<IOptions<LlamaRepositoryOptions>>().Should().NotBeNull();
        factory.Services.GetService<IOptions<LLamaModelOptions>>().Should().NotBeNull();
        factory.Services.GetService<IOptions<InferenceOptions>>().Should().NotBeNull();

        var options = factory.Services.GetRequiredService<IOptions<LLamaModelOptions>>().Value;
        //options.ModelPath.Should().Be("/projects/AI/LlamaModels/llama-2-7b.Q4_0.gguf");
        options.ModelPath.Should().Contain("/projects/AI/LlamaModels/");
        options.ModelPath.Should().EndWith(".gguf");

        var iOptions = factory.Services.GetRequiredService<IOptions<InferenceOptions>>().Value;
        iOptions.AntiPrompts.Single().Should().Be("User:");
        iOptions.MaxTokens.Should().Be(1024);

        var openApiOptions = factory.Services.GetRequiredService<IOptions<OpenApiOptions>>().Value;

        openApiOptions.Info.Should().NotBeNull();
        openApiOptions.SecurityScheme.Should().NotBeNull();
        openApiOptions.SecurityRequirement.Should().NotBeNull();
        openApiOptions.Info!.Description.Should().Be("LLamaSharpApp.WebAPI");
        openApiOptions.Info.Title.Should().Be("Generic model requests");
        openApiOptions.Info.Version.Should().Be("v42");

        openApiOptions.SecurityScheme!.Description.Should().Be("hello - Enter JWT Bearer token **_only_*");
        openApiOptions.SecurityScheme.Type.Should().Be(SecuritySchemeType.ApiKey);
        openApiOptions.SecurityScheme.In.Should().Be(ParameterLocation.Header);

        //openApiOptions.SecurityRequirement
        //var s = openApiOptions.ToJson();
        //s.Should().NotBeNullOrEmpty();
        //Log.Logger.Debug(openApiOptions!.Info!.Title!);
    }
}
