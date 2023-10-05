using AI.Test.Support.DockerSupport.Testcontainers.Qdrant;

using DotNet.Testcontainers.Configurations;

using FluentAssertions;

using QdrantCSharp;
using QdrantCSharp.Enums;
using QdrantCSharp.Models;

using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests.QdrantSupport;

/// <summary>
/// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
/// </summary>
public sealed class TestOfQdrantWithVolumeMountedConfiguration : IAsyncLifetime
{
    private readonly ITestOutputHelper output;
    private readonly QdrantContainer quadrantContainer;

    private const string CollectionName = "qdrant-test-collection";
    private const int VectorSize = 4;

    public TestOfQdrantWithVolumeMountedConfiguration(ITestOutputHelper output)
    {
        this.output = output;
        quadrantContainer = new QdrantBuilder()
            .WithContainerName("qdrant-testcontainer")
            .WithVolume("/temp/testcontainer_qdrant_storage", "/qdrant/storage", AccessMode.ReadWrite)
            .Build();
    }

    public Task InitializeAsync()
    {
        return quadrantContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return quadrantContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CreateCollectionAndFindIt_UsingMountedVolumeOptions()
    {
        var client = new QdrantHttpClient(quadrantContainer.GetConnectionUrl(), string.Empty);
        var response = await client.CreateCollection(CollectionName, new VectorParams(size: 4, distance: Distance.DOT));
        response.Result.Should().BeTrue();
        output.WriteLine($"Created Collection {CollectionName}");
        var result = await client.GetCollection(CollectionName);
        result.Should().NotBeNull();
        result.Status.Should().Be("ok");
        output.WriteLine($"Verified Collection {CollectionName}");
    }
}
