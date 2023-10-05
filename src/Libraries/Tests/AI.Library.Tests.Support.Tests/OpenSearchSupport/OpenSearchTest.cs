using FluentAssertions;
using OpenSearch.Client;
using OpenSearch.Net;
using Xunit.Abstractions;

namespace AI.Library.Tests.Support.Tests.OpenSearchSupport;

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

    [Fact]
    public void CreateStudentIndex()
    {
        //admin : admin
        // Given
        var node = new Uri("https://localhost:9200");
        var config = new ConnectionSettings(node)
                .DefaultIndex("students")
                .BasicAuthentication(userName, password)
                .ServerCertificateValidationCallback((a, b, c, d) => true)
            ;
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
            .ServerCertificateValidationCallback((a, b, c, d) => true)
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
        var node = new Uri("https://localhost:9200/");
        var config = new ConnectionSettings(node)
                .DefaultIndex("students")
                .BasicAuthentication(userName, password)
                .ServerCertificateValidationCallback((a, b, c, d) => true)
            ;
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
        var config = new ConnectionSettings(node)
                .DefaultIndex("students")
                .BasicAuthentication(userName, password)
                .ServerCertificateValidationCallback((a, b, c, d) => true)
            ;

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