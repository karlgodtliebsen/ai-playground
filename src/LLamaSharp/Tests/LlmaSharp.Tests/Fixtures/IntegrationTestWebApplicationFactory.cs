using System.Security.Claims;

using AI.Test.Support;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

namespace LlamaSharp.Tests.Fixtures;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    public IntegrationTestWebApplicationFactory()
    {
        Log.CloseAndFlush();
    }

    public string UserId { get; set; } = Guid.NewGuid().ToString("N");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        builder.UseContentRoot(path);
        builder.UseEnvironment(Environments.Development);
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHttpContextAccessor>(CreateHttpContext().Object);
            services.AddSingleton<ILoggerFactory, XUnitTestLoggerFactory>();
            //services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        });
        builder.ConfigureLogging((options) =>
        {

        });
    }

    private Mock<IHttpContextAccessor> CreateHttpContext()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("sub", UserId),
        }, "TestAuthentication"));

        var defaultHttpContext = new Mock<HttpContext>();
        defaultHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        var ctxAccessor = new Mock<IHttpContextAccessor>();
        ctxAccessor.Setup(x => x.HttpContext).Returns(defaultHttpContext.Object);
        return ctxAccessor;
    }
}

