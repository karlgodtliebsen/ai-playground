using AI.Test.Support.Fixtures;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AI.Library.Tests.Support.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IntegrationTestWebApplicationFactory : IntegrationTestWebApplicationFactory<Program>
{

    protected override void ConfigureTestServices(IServiceCollection services, IConfiguration cfg)
    {
    }

}

