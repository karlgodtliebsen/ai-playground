using AI.Library.Configuration;
using AI.Test.Support.DockerSupport.Testcontainers.Qdrant;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Testcontainers.PostgreSql;

namespace AI.Test.Support.DockerSupport;

public class TestFixtureBaseWithDocker : TestFixtureBase
{
    private bool useDocker = false;
    private bool useQdrantDocker = false;
    private bool usePostgreSqlDocker = false;

    public TestContainerDockerLauncher? Launcher { get; private set; } = default!;
    public HostApplicationFactory? Factory { get; private set; } = default!;

    protected QdrantContainer? QuadrantContainer { get; private set; } = default!;

    protected PostgreSqlContainer? PostgreSqlContainer { get; private set; } = default!;

    /// <summary>
    /// abstract constructor
    /// </summary>
    protected TestFixtureBaseWithDocker()
    {

    }


    protected void AddDockerSupport(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TestContainerDockerLauncher>();
        var section = configuration.GetSection(DockerLaunchOptions.SectionName);
        services.AddOptions<DockerLaunchOptions>().Bind(section);
    }

    protected void AddPostgreSqlSupport(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgreSql(configuration);
    }
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        if (useDocker)
        {
            AddDockerSupport(services, configuration!);
        }
        if (usePostgreSqlDocker)
        {
            AddPostgreSqlSupport(services, configuration!);
        }
    }

    public TestFixtureBase WithQdrantSupport()
    {
        useQdrantDocker = true;
        return this;
    }

    public TestFixtureBase WithPostgreSqlSupport()
    {
        usePostgreSqlDocker = true;
        return this;
    }

    public TestFixtureBase WithDockerSupport()
    {
        useDocker = true;
        return this;
    }

    public override HostApplicationFactory Build()
    {
        if (useQdrantDocker)
        {
            QuadrantContainer = new QdrantBuilder().Build();
        }
        if (usePostgreSqlDocker)
        {
            PostgreSqlContainer = new PostgreSqlBuilder().Build();
        }

        Factory = BuildFactory();
        if (useDocker)
        {
            Factory = Factory.WithDockerSupport(out var launch);
            Launcher = launch!;
        }
        return Factory;
    }

    public async Task InitializeAsync()
    {
        ArgumentNullException.ThrowIfNull(Factory);
        if (useQdrantDocker && QuadrantContainer is not null)
        {
            await QuadrantContainer.StartAsync().ConfigureAwait(false);
            Factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value.Url = QuadrantContainer.GetConnectionUrl();
        }

        if (usePostgreSqlDocker && PostgreSqlContainer is not null)
        {
            await PostgreSqlContainer.StartAsync().ConfigureAwait(false);
            Factory.Services.GetRequiredService<IOptions<PostgreSqlOptions>>().Value.ConnectionString = PostgreSqlContainer.GetConnectionString();
        }

    }

    public async Task DisposeAsync()
    {
        if (useQdrantDocker && QuadrantContainer is not null)
        {
            await QuadrantContainer.DisposeAsync();
        }
        if (usePostgreSqlDocker && PostgreSqlContainer is not null)
        {
            await PostgreSqlContainer.DisposeAsync();
        }
    }
}
