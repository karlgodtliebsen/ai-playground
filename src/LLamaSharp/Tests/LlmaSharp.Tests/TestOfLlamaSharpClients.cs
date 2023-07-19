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
        factory.Dispose();
    }


    [Fact]
    public async Task VerifyThatUserIdProviderFindsUserSubject()
    {
        using var scope = factory.Services.CreateScope();
        var userProvider = scope.ServiceProvider.GetRequiredService<IUserIdProvider>();
        userProvider.UserId.Should().Be(factory.UserId);
    }

    //TODO: extend the tests to verify that the clients work as expected

}
