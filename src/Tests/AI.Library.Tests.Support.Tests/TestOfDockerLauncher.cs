using System.Data.Common;

using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Serilog;

using Xunit.Abstractions;


namespace AI.Library.Tests.Support.Tests;

[Collection("Docker Launch Collection")]
public class TestOfDockerLauncher : IAsyncLifetime
{
    private readonly ITestOutputHelper output;

    private readonly DockerLaunchTestFixture fixture;
    private HostApplicationFactory factory;

    private TestContainerDockerLauncher? launcher = default!;

    public TestOfDockerLauncher(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {

        this.output = output;
        this.fixture = fixture;
        factory = fixture.WithOutputLogSupport(output).WithDockerSupport().Build(out launcher);
    }


#pragma warning disable xUnit1013
    public async Task InitializeAsync()
#pragma warning restore xUnit1013
    {
        await launcher?.StartAsync(CancellationToken.None)!;
    }

#pragma warning disable xUnit1013
    public async Task DisposeAsync()
#pragma warning restore xUnit1013
    {
        await launcher?.StopAsync(CancellationToken.None)!;
    }

    /// <summary>
    /// <a href="https://github.com/testcontainers/testcontainers-dotnet/blob/develop/src/Testcontainers.PostgreSql/PostgreSqlBuilder.cs">PostgreSqlBuilder</a>
    /// </summary>
    [Fact]
    public void RunDockerLaunchTest()
    {
        factory.Should().NotBeNull();
        launcher.Should().NotBeNull();
        var services = factory.Services;
        services.Should().NotBeNull();
        var logger = services.GetRequiredService<ILogger>();
        logger.Should().NotBeNull();
        logger.Information("Hello World");

        string connectionString = fixture.DatabaseConnectionString;
        using DbConnection connection = new NpgsqlConnection(connectionString);
        using DbCommand command = new NpgsqlCommand();
        connection.Open();
        command.Connection = connection;
        command.CommandText = "SELECT 1";
    }
}


