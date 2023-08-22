using System.Data.Common;

using Npgsql;

using Testcontainers.PostgreSql;

namespace AI.Library.Tests.Support.Tests;


/// <summary>
/// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
/// </summary>
public sealed class PostgreSqlContainerTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return postgreSqlContainer.DisposeAsync().AsTask();
    }

    //https://github.com/testcontainers/testcontainers-dotnet/blob/develop/src/Testcontainers.PostgreSql/PostgreSqlConfiguration.cs
    //PostgreSqlConfiguration 

    [Fact]
    public void ExecuteCommand()
    {
        var connectionString = postgreSqlContainer.GetConnectionString();
        using DbConnection connection = new NpgsqlConnection(connectionString);
        using DbCommand command = new NpgsqlCommand();
        connection.Open();
        command.Connection = connection;
        command.CommandText = "SELECT 1";
    }
}

