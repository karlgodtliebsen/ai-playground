using AI.Library.Configuration;
using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenSearch.Client;
using OpenSearch.Net;

using Testcontainers.Elasticsearch;

using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests.OpenSearchSupport;

/// <summary>
/// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
/// </summary>
[Collection("Docker Launch Collection")]
public sealed class TestOfOpenSearchSupport : IAsyncLifetime
{
    private readonly ITestOutputHelper output;
    private readonly HostApplicationFactory factory;
    private readonly DockerLaunchTestFixture fixture;
    private const string userName = "admin";
    private const string password = "admin";

    public TestOfOpenSearchSupport(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {
        this.output = output;
        this.fixture = fixture;
        factory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithOpenSearchSupport().Build();
    }

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    [Fact]
    public void ExecuteStudentIndex()
    {
        var openSearchOptions = factory.Services.GetRequiredService<IOptions<OpenSearchOptions>>().Value;
        openSearchOptions.Should().NotBeNull();

        var node = new Uri(openSearchOptions.ConnectionString!);
        var config = new ConnectionSettings(node)
            .DefaultIndex("students")
            .BasicAuthentication(userName, password);
        var client = new OpenSearchClient(config);

        var student = new Student { Id = 100, FirstName = "Paulo", LastName = "Santos", Gpa = 3.93, GradYear = 2021 };
        var response = client.Index(student, i => i.Index("students"));
        output.WriteLine(response.DebugInformation);

        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();

        student = new Student { Id = 142, FirstName = "Paulo", LastName = "Santos", Gpa = 3.93, GradYear = 2021 };
        var response2 = client.Index(new IndexRequest<Student>(student, "students"));
        output.WriteLine(response2.DebugInformation);
        response2.Should().NotBeNull();
        response2.IsValid.Should().BeTrue();
    }
}

public class Student
{
    public int Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int GradYear { get; init; }
    public double Gpa { get; init; }
}

public class QueryRequest
{
    public Query Query { get; init; } = new();

}

public class Query
{
    public Match Match { get; init; } = new();

}

public class Match
{
    public string LastName { get; init; }
}

/*
 {
  "query" : {
    "match": {
      "lastName": "Santos"
    }
  }
}

*/



public sealed class ElasticsearchContainerTest : IAsyncLifetime
{
    private readonly ITestOutputHelper output;
    private readonly ElasticsearchContainer elasticsearchContainer = new ElasticsearchBuilder().Build();
    private const string userName = "admin";
    private const string password = "admin";
    public ElasticsearchContainerTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    public Task InitializeAsync()
    {
        return elasticsearchContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return elasticsearchContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void PingReturnsValidResponse()
    {
        // Given
        var connectionString = elasticsearchContainer.GetConnectionString();
        var node = new Uri(connectionString);
        var config = new ConnectionSettings(node)
            .BasicAuthentication(userName, password);
        var client = new OpenSearchClient(config);

        // When
        var response = client.Ping();
        output.WriteLine(response.DebugInformation);
        // Then
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
    }
}

//https://opensearch.org/docs/latest/clients/OSC-dot-net/

public sealed class OpenSearchTest
{
    private readonly ITestOutputHelper output;
    private const string userName = "admin";
    private const string password = "admin";

    public OpenSearchTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void PingReturnsValidResponse()
    {
        // Given
        var node = new Uri("https://localhost:9200");
        var config = new ConnectionSettings(node)
            .BasicAuthentication(userName, password);
        var client = new OpenSearchClient(config);

        // When
        var response = client.Ping();
        output.WriteLine(response.DebugInformation);
        // Then
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateStudentIndex()
    {
        //admin : admin
        // Given
        var node = new Uri("https://localhost:9200");
        var config = new ConnectionSettings(node)
            .DefaultIndex("students")
            .BasicAuthentication(userName, password);
        var client = new OpenSearchClient(config);
        // When
        var student = new Student { Id = 142, FirstName = "Paulo", LastName = "Santos", Gpa = 3.93, GradYear = 2021 };
        var response = client.Index(new IndexRequest<Student>(student, "students"));
        output.WriteLine(response.DebugInformation);
        // Then
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
    }
    [Fact]
    public void CreateStudentIndexUsingLowlevelClient()
    {
        // Given
        var uri = new Uri("https://localhost:9200/");
        var connectionPool = new SingleNodeConnectionPool(uri);
        var settings = new ConnectionSettings(connectionPool)
            .DefaultIndex("students")
            .EnableHttpCompression()
            .PrettyJson()
            .DefaultFieldNameInferrer(f => f.ToLower())
            .BasicAuthentication(userName, password);

        var client = new OpenSearchLowLevelClient(settings);

        // When
        var student = new Student { Id = 4242, FirstName = "Paulo", LastName = "Santos", Gpa = 3.93, GradYear = 2021 };
        var response = client.Index<StringResponse>("students", "4242", PostData.Serializable(student));
        output.WriteLine(response.DebugInformation);
        // Then
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateStudentIndexAsync()
    {
        // Given
        var node = new Uri("https://localhost:9200");
        var config = new ConnectionSettings(node)
            .DefaultIndex("students")
            .BasicAuthentication(userName, password);
        var client = new OpenSearchClient(config);
        // When
        var student = new Student { Id = 142, FirstName = "Paulo", LastName = "Santos", Gpa = 3.93, GradYear = 2021 };
        var response = await client.IndexAsync(new IndexRequest<Student>(student, "students"), CancellationToken.None);

        // var response = client.IndexAsync<StringResponse>("students", "100", PostData.Serializable(student));

        output.WriteLine(response.DebugInformation);
        // Then
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
    }



    [Fact]
    public void CreateMultipleStudentsIndex()
    {
        // Given
        var node = new Uri("https://localhost:9200");
        var config = new ConnectionSettings(node).DefaultIndex("students")
            .BasicAuthentication(userName, password);

        var client = new OpenSearchClient(config);

        // When

        var studentArray = new Student[]
        {
            new() {Id = 242, FirstName = "Shirley", LastName = "Rodriguez", Gpa = 3.91, GradYear = 2019},
            new() {Id = 342, FirstName = "Nikki", LastName = "Wolf", Gpa = 3.87, GradYear = 2020}
        };

        var response = client.IndexMany(studentArray, "students");
        output.WriteLine(response.DebugInformation);

        // Then
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
    }


}
