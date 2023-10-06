using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.Configuration;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using QdrantCSharp;
using QdrantCSharp.Enums;
using QdrantCSharp.Models;

using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests.QdrantSupport;

[Collection("Docker Launch Collection")]
public sealed class TestOfQdrantWithDefaultConfigurationAndUsingQdrantSupport : IAsyncLifetime
{
    private readonly ITestOutputHelper output;
    private readonly HostApplicationFactory factory;
    private readonly DockerLaunchTestFixture fixture;

    private const string CollectionName = "qdrant-testcontainer-collection";
    private const int VectorSize = 4;

    public TestOfQdrantWithDefaultConfigurationAndUsingQdrantSupport(ITestOutputHelper output, DockerLaunchTestFixture fixture)
    {
        this.output = output;
        this.fixture = fixture;
        factory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
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
    public async Task CreateCollectionAndFindIt_UsingDefaultOptions()
    {
        var connectionUrl = factory.Services.GetRequiredService<IOptions<QdrantOptions>>().Value.Endpoint!;
        var client = new QdrantHttpClient(connectionUrl, string.Empty);
        var response = await client.CreateCollection(CollectionName, new VectorParams(size: VectorSize, distance: Distance.DOT));
        response.Result.Should().BeTrue();
        output.WriteLine($"Created Collection {CollectionName}");
        var result = await client.GetCollection(CollectionName);
        result.Should().NotBeNull();
        result.Status.Should().Be("ok");
        output.WriteLine($"Verified Collection {CollectionName}");
    }
}
