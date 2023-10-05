using System.Net.Http.Headers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Test.Support.Fixtures;

/// <summary>
/// https://gunnarpeipman.com/aspnet-core-integration-tests-users-roles/
/// </summary>
public static class TestFactoryExtensions
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", op => { });

                services.AddScoped(_ => claimsProvider);
            });
        });
    }

    public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
    {
        var client = factory
            .WithAuthentication(claimsProvider)
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        return client;
    }
}
