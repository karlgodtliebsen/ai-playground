using System.Data.Common;

using Npgsql;

using Testcontainers.PostgreSql;

namespace AI.Library.Tests.Support.Tests;

public sealed class PostgreSqlContainerTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }


    [Fact]
    public void ExecuteCommand()
    {
        using (DbConnection connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString()))
        {
            using (DbCommand command = new NpgsqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT 1";
            }
        }
    }
}

