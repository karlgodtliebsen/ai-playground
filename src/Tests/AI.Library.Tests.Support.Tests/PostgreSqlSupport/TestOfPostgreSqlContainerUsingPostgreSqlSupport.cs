using System.Data.Common;

using AI.Library.Configuration;
using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Options;

using Npgsql;

using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests.PostgreSqlSupport;

/// <summary>
/// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
/// </summary>
[Collection("Docker Launch Collection")]
public sealed class TestOfPostgreSqlContainerUsingPostgreSqlSupport : IAsyncLifetime
{
    private readonly ITestOutputHelper output;
    private readonly HostApplicationFactory factory;
    private readonly DockerLaunchTestFixture fixture;

    public TestOfPostgreSqlContainerUsingPostgreSqlSupport(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {
        this.output = output;
        this.fixture = fixture;
        factory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithPostgreSqlSupport().Build();

    }

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    //https://github.com/testcontainers/testcontainers-dotnet/blob/develop/src/Testcontainers.PostgreSql/PostgreSqlConfiguration.cs
    //PostgreSqlConfiguration 

    [Fact]
    public void ExecuteCommand()
    {
        var connectionString = factory.Services.GetRequiredService<IOptions<PostgreSqlOptions>>().Value.ConnectionString!;
        using DbConnection connection = new NpgsqlConnection(connectionString);
        using DbCommand command = new NpgsqlCommand();
        connection.Open();
        command.Connection = connection;
        command.CommandText = "SELECT 1";
    }
}
