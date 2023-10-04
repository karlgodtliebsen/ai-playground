using FluentAssertions;
using OpenSearch.Client;
using Testcontainers.Elasticsearch;
using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests.OpenSearchSupport;

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
                .BasicAuthentication(userName, password)
                .ServerCertificateValidationCallback((a, b, c, d) => true)
            ;
        var client = new OpenSearchClient(config);

        // When
        var response = client.Ping();
        output.WriteLine(response.DebugInformation);
        // Then
        response.Should().NotBeNull();
        response.IsValid.Should().BeTrue();
    }
}