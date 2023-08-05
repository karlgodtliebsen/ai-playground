using AI.Library.HttpUtils.LibraryConfiguration;

using FluentAssertions;

using LlamaSharp.Tests.Fixtures;

using LLamaSharp.Domain.Configuration;
using LLamaSharp.Domain.Domain.Repositories;
using LLamaSharp.Domain.Domain.Services;

using LLamaSharpApp.WebAPI.Controllers.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace LlamaSharp.Tests;

public sealed class TestOfConfiguration : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly IntegrationTestWebApplicationFactory factory;

    public TestOfConfiguration(IntegrationTestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    public void Dispose()
    {
        factory.Dispose();
    }

    [Fact]
    public void EnsureThatServiceConfigurationIsValid()
    {
        factory.Services.GetService<ILogger>().Should().NotBeNull();

        factory.Services.GetService<ILoggerFactory>().Should().NotBeNull();
        factory.Services.GetService<ILogger<object>>().Should().NotBeNull();

        factory.Services.GetService<ILlamaModelFactory>().Should().NotBeNull();
        factory.Services.GetService<IOptionsService>().Should().NotBeNull();
        factory.Services.GetService<IChatDomainService>().Should().NotBeNull();
        factory.Services.GetService<IEmbeddingsService>().Should().NotBeNull();
        factory.Services.GetService<IExecutorService>().Should().NotBeNull();
        factory.Services.GetService<IModelStateRepository>().Should().NotBeNull();
        factory.Services.GetService<IUsersStateRepository>().Should().NotBeNull();
        //Scoped service registrations
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetService<IUserIdProvider>().Should().NotBeNull();

        //Options
        factory.Services.GetService<IOptions<LlamaRepositoryOptions>>().Should().NotBeNull();
        factory.Services.GetService<IOptions<LlamaModelOptions>>().Should().NotBeNull();
        factory.Services.GetService<IOptions<InferenceOptions>>().Should().NotBeNull();

        var options = factory.Services.GetRequiredService<IOptions<LlamaModelOptions>>().Value;
        options.ModelPath.Should().Be("/projects/AI/LlamaModels/llama-2-7b.ggmlv3.q8_0.bin");

        var iOptions = factory.Services.GetRequiredService<IOptions<InferenceOptions>>().Value;
        iOptions.AntiPrompts.Single().Should().Be("User:");
        iOptions.MaxTokens.Should().Be(1024);

        var openApiOptions = factory.Services.GetRequiredService<IOptions<OpenApiOptions>>().Value;

        openApiOptions.Info.Should().NotBeNull();
        openApiOptions.SecurityScheme.Should().NotBeNull();
        openApiOptions.SecurityRequirement.Should().NotBeNull();
        openApiOptions.Info.Description.Should().Be("LLamaSharpApp.WebAPI");
        openApiOptions.Info.Title.Should().Be("Generic model requests");
        openApiOptions.Info.Version.Should().Be("v42");

        openApiOptions.SecurityScheme.Description.Should().Be("hello - Enter JWT Bearer token **_only_*");
        openApiOptions.SecurityScheme.Type.Should().Be(SecuritySchemeType.ApiKey);
        openApiOptions.SecurityScheme.In.Should().Be(ParameterLocation.Header);

        //openApiOptions.SecurityRequirement
        //var s = openApiOptions.ToJson();
        //s.Should().NotBeNullOrEmpty();
        //Log.Logger.Debug(openApiOptions!.Info!.Title!);

    }

}
