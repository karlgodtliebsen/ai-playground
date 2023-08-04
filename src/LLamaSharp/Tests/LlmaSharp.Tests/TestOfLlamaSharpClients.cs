using FluentAssertions;

using LlamaSharp.Tests.Fixtures;

using LLamaSharpApp.WebAPI.Controllers.Services;

using Microsoft.Extensions.DependencyInjection;

namespace LlamaSharp.Tests;

public class TestOfLlamaSharpClients : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly IntegrationTestWebApplicationFactory factory;

    public TestOfLlamaSharpClients(IntegrationTestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    public void Dispose()
    {
        //factory.Dispose();//Code smell: really annoying that this messes up the test runner
    }


    [Fact]
    public async Task VerifyThatUserIdProviderFindsUserSubject()
    {
        using var scope = factory.Services.CreateScope();
        var userProvider = scope.ServiceProvider.GetRequiredService<IUserIdProvider>();
        userProvider.UserId.Should().Be(factory.UserId);
    }

    [Fact]
    public async Task VerifyThatHttpClientCanHealthCheckController()
    {
        var claimsProvider = TestClaimsProvider.WithAdministratorClaims();
        using var client = factory.CreateClientWithTestAuth(claimsProvider);
        var response = await client.GetAsync("https://localhost/health");
        response.EnsureSuccessStatusCode();
        var status = await response.Content.ReadAsStringAsync();
        status.Should().NotBeNull();
        status.Should().Be("Healthy");
    }

    [Fact]
    public async Task VerifyThatHttpClientCanCallControllerAndFetchPromptTemplates()
    {
        var claimsProvider = TestClaimsProvider.WithAdministratorClaims();
        using var client = factory.CreateClientWithTestAuth(claimsProvider);
        var response = await client.GetAsync($"https://localhost/api/llama/configuration/prompt-templates", CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(CancellationToken.None);
        text.Should().NotBeNull();
    }

    [Fact]

    public async Task VerifyThatLLamaClientCanCallControllerAndFetchPromptTemplates()
    {
        var client = factory.Services.GetRequiredService<ILLamaClient>();
        var response = await client.GetPromptTemplatesAsync(CancellationToken.None);
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task VerifyThatLLamaClientCanHealthCheckController()
    {
        var client = factory.Services.GetRequiredService<ILLamaClient>();
        var status = await client.CheckHealthEndpoint(CancellationToken.None);
        status.Should().NotBeNull();
        status.Should().Be("Healthy");
    }
}




