using System.Net;
using System.Security.Claims;

using AI.Library.Configuration;
using AI.Test.Support.Fixtures;
using AI.Test.Support.Logging;

using LlamaSharp.Tests.Utils;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

using Polly;
using Polly.Extensions.Http;

using Serilog;

using Xunit.Abstractions;

namespace LlamaSharp.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private IConfiguration configuration;
    public ITestOutputHelper Output { get; protected set; }
    public ILogger Logger { get; protected set; }

    public string UserId { get; set; } = Guid.NewGuid().ToString();

    /// <inheritdoc />
    public IntegrationTestWebApplicationFactory()
    {
        Log.CloseAndFlush();
    }

    /// <summary>
    /// Post Build Setup of Logging that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    public void Setup(ITestOutputHelper output)
    {
        Output = output;
        Services.GetService<ILogger>();//kickstarts the building of the IntegrationTestWebApplicationFactory. Something is probably wrong here, it should be done in another way??
        ConfigureLogging(output);
    }

    private void ConfigureLogging(ITestOutputHelper output)
    {
        var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(configuration);
        cfg = cfg.WriteTo.TestOutput(output);
        Log.Logger = cfg.CreateLogger();
        Logger = Services.GetRequiredService<ILogger>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        builder.UseContentRoot(path);
        builder.UseEnvironment("IntegrationTests");
        base.ConfigureWebHost(builder);
        builder.ConfigureAppConfiguration((x, cfg) =>
        {
            this.configuration = x.Configuration;
        });

        builder.ConfigureTestServices(services =>
         {
             services.AddSingleton(CreateHttpContext());
             services.AddSingleton<ILoggerFactory, XUnitTestLoggerFactory>();

             services.AddTransient<ILogger>((_) => Log.Logger);

             const string endpointUrl = "https://localhost";
             var options = new LlamaClientOptions()
             {
                 Endpoint = endpointUrl,
             };
             services.AddSingleton<IOptions<LlamaClientOptions>>(new OptionsWrapper<LlamaClientOptions>(options));
             services.AddScoped(_ => TestClaimsProvider.WithAdministratorClaims());

             LLamaConfigurationClient ConfigClient(HttpClient c, IServiceProvider sp)
             {
                 var claimsProvider = sp.GetRequiredService<TestClaimsProvider>();
                 var client = this.CreateClientWithTestAuth(claimsProvider);
                 return new LLamaConfigurationClient(client, sp.GetRequiredService<IOptions<LlamaClientOptions>>(), sp.GetRequiredService<ILogger>());
             }

             services.AddHttpClient<ILLamaConfigurationClient, LLamaConfigurationClient>(ConfigClient)
                 .AddPolicyHandler(GetCircuitBreakerPolicyForCustomerServiceNotFound())
             ;

             LLamaCompositeOperationsClient CompositeClient(HttpClient c, IServiceProvider sp)
             {
                 var claimsProvider = sp.GetRequiredService<TestClaimsProvider>();
                 var client = this.CreateClientWithTestAuth(claimsProvider);
                 return new LLamaCompositeOperationsClient(client, sp.GetRequiredService<IOptions<LlamaClientOptions>>(), sp.GetRequiredService<ILogger>());
             }

             services.AddHttpClient<ILLamaCompositeOperationsClient, LLamaCompositeOperationsClient>(CompositeClient)
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
        return Policy
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

