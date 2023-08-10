using System.Net;
using System.Security.Claims;

using AI.Test.Support.Logging;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using Polly;
using Polly.Extensions.Http;

using Serilog;

namespace LlamaSharp.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    public IntegrationTestWebApplicationFactory()
    {
        Log.CloseAndFlush();
    }

    public string UserId { get; set; } = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        builder.UseContentRoot(path);
        builder.UseEnvironment("IntegrationTests");
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IHttpContextAccessor>(CreateHttpContext());
            services.AddSingleton<ILoggerFactory, XUnitTestLoggerFactory>();
            const string endpointUrl = "https://localhost";
            var options = new LlamaClientOptions()
            {
                Endpoint = endpointUrl,
            };
            services.AddSingleton<IOptions<LlamaClientOptions>>(new OptionsWrapper<LlamaClientOptions>(options));
            services.AddScoped(_ => TestClaimsProvider.WithAdministratorClaims());
            Func<HttpClient, IServiceProvider, LLamaClient> f = (c, sp) =>
            {
                var claimsProvider = sp.GetRequiredService<TestClaimsProvider>();
                var client = this.CreateClientWithTestAuth(claimsProvider);
                return new LLamaClient(client, sp.GetRequiredService<IOptions<LlamaClientOptions>>(), sp.GetRequiredService<ILogger>());
            };

            services.AddHttpClient<ILLamaClient, LLamaClient>(f)
                .AddPolicyHandler(GetCircuitBreakerPolicyForCustomerServiceNotFound())
            ;
        });
    }

    private IHttpContextAccessor CreateHttpContext()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("sub", UserId),
        }, "TestAuthentication"));

        var defaultHttpContext = Substitute.For<HttpContext>();
        defaultHttpContext.User.Returns(claimsPrincipal);
        IHttpContextAccessor? ctxAccessor = Substitute.For<IHttpContextAccessor>();
        ctxAccessor.HttpContext.Returns(defaultHttpContext);
        return ctxAccessor;
    }
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicyForCustomerServiceNotFound()
    {
        return Polly.Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound)
                .CircuitBreakerAsync(1, TimeSpan.FromMicroseconds(1))
            ;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}

