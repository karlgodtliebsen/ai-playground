using AI.Library.Configuration;
using AI.Library.Tests.Support.Tests.Fixtures;
using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenSearch.Client;

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
            .BasicAuthentication(userName, password)
            .ServerCertificateValidationCallback((a, b, c, d) => true)
            ;
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

//Tip: Use anonymous types to create a query

//https://opensearch.org/docs/latest/clients/OSC-dot-net/
