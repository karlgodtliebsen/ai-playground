using System.Net;
using System.Security.Claims;

using AI.Library.Configuration;
using AI.Test.Support.DockerSupport;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Polly;
using Polly.Extensions.Http;

using Serilog;

using Xunit.Abstractions;

namespace AI.Test.Support.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public abstract class IntegrationTestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    private IConfiguration? configuration;
    private ITestOutputHelper? outputHelper = default!;
    private bool useDocker = false;

    public ILogger Logger { get; private set; } = default!;
    public Microsoft.Extensions.Logging.ILogger MsLogger { get; private set; } = default!;

    public string UserId { get; set; } = "01HBPGFX6ESEK2NMZKJ19KDCAA";//Ulid.NewUlid().ToString();
    public string Environment { get; set; } = "IntegrationTests";

    private TestContainerDockerLauncher? launcher = default!;

    /// <inheritdoc />
    public IntegrationTestWebApplicationFactory()
    {
        Log.CloseAndFlush();
        //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0
        var env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        if (env is not null)
        {
            Environment = env;
        }
    }
    /// <summary>
    /// Post Build Setup of Logging that depends on ITestOutputHelper
    /// </summary>
    /// <param name="output"></param>
    public IntegrationTestWebApplicationFactory<TEntryPoint> WithOutputLogSupport(ITestOutputHelper output)
    {
        this.outputHelper = output;
        return this;
    }

    public IntegrationTestWebApplicationFactory<TEntryPoint> WithDockerSupport()
    {
        useDocker = true;
        return this;
    }

    public T Build<T>() where T : IntegrationTestWebApplicationFactory<TEntryPoint>
    {
        Services.GetService<ILogger>();     //triggers the ConfigureWebHost. Nice lazy activation
        ConfigureLogging();
        if (useDocker)
        {
            StartDocker();
        }
        return (T)this;
    }

    private void StartDocker()
    {
        launcher = Services.GetRequiredService<TestContainerDockerLauncher>();
        launcher.Start();
    }

    public new void Dispose()
    {
        launcher?.Stop();
        launcher = default;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = Path.GetDirectoryName(typeof(TEntryPoint).Assembly.Location);
        builder.UseContentRoot(path!);
        builder.UseEnvironment(Environment);
        base.ConfigureWebHost(builder);
        builder.ConfigureAppConfiguration((ctx, _) =>
        {
            this.configuration = ctx.Configuration;
        });

        builder.ConfigureTestServices(services =>
         {
             services.AddSingleton(CreateHttpContext());
             SetupLogging(services);
             services.AddScoped(_ => TestClaimsProvider.WithAdministratorClaims());
             if (useDocker && configuration is not null)
             {
                 SetupDockerSupport(services, configuration);
             }
             if (configuration is not null)
             {
                 ConfigureTestServices(services, configuration!);
             }
         });
    }

    protected virtual void ConfigureTestServices(IServiceCollection services, IConfiguration cfg)
    {
    }

    private void ConfigureLogging()
    {
        Logger = Services.GetRequiredService<ILogger>();
        var loggerFactory = Services.GetRequiredService<ILoggerFactory>();
        MsLogger = loggerFactory.CreateLogger("Test");
    }

    private void SetupDockerSupport(IServiceCollection services, IConfiguration cfg)
    {
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = cfg.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }

    private void SetupLogging(IServiceCollection services)
    {
        if (outputHelper is not null)
        {
            services.AddLogging(logging =>
            {
                if (configuration is null)
                {
                    throw new InvalidOperationException("Configuration is null");
                }
                var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(configuration);
                cfg = cfg.WriteTo.TestOutput(outputHelper);
                Log.Logger = cfg.CreateLogger();
                services.AddSingleton<Serilog.ILogger>(Log.Logger);
                services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("test"));
                var useScopes = logging.UsesScopes();
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                services.AddSingleton<ILoggerFactory, XUnitTestMsLoggerFactory>();
                services.AddSingleton<ILoggerProvider>(new XUnitTestMsLoggerProvider(outputHelper, useScopes));
            });
        }
        else
        {
            services.AddLogging(logging =>
            {
                if (configuration is null)
                {
                    throw new InvalidOperationException("Configuration is null");
                }
                var cfg = Observability.CreateLoggerConfigurationUsingAppSettings(configuration);
                cfg = cfg.WriteTo.TestOutput(outputHelper);
                Log.Logger = cfg.CreateLogger();
                services.AddSingleton<Serilog.ILogger>(Log.Logger);
                services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("test"));
                var useScopes = logging.UsesScopes();
                logging.AddConsole();
                logging.AddDebug();
                services.AddSingleton<ILoggerFactory, XUnitTestMsLoggerFactory>();
                services.AddSingleton<ILoggerProvider>(new XUnitConsoleMsLoggerProvider(Console.Out, useScopes));
            });
        }
    }

    protected IHttpContextAccessor CreateHttpContext()
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
    protected static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicyForCustomerServiceNotFound()
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

