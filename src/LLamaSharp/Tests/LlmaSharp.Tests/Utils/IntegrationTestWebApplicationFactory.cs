using System.Security.Claims;

using LLamaSharpApp.WebAPI.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Serilog;

namespace LlamaSharp.Tests.Utils;

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
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddSingleton<IHttpContextAccessor>(CreateHttpContext().Object);
            services.AddWebApiConfiguration();
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
