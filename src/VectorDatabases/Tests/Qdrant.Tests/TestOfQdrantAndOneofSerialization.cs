using System.Text.Json.Serialization;

using FluentAssertions;

using OneOf;

using Xunit.Abstractions;

namespace Qdrant.Tests;


public class TestOfQdrantAndOneofSerialization
{
    private readonly ITestOutputHelper output;
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public TestOfQdrantAndOneofSerialization(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact(Skip = "Awaiting solution to json serialize/Deserialize OnfOf")]
    public async Task VerifyOneOf()
    {

        var sut1 = new ScoredPointTester()
        {
            Id = Guid.NewGuid(),
        };

        var sut2 = new ScoredPointTester()
        {
            Id = 42
        };


        var s_Sut1 = JsonSerializer.Deserialize<ScoredPointTester>(JsonSerializer.Serialize(sut1, serializerOptions));
        var s_Sut2 = JsonSerializer.Deserialize<ScoredPointTester>(JsonSerializer.Serialize(sut2, serializerOptions));

        s_Sut1.Id.Should().Be(sut1.Id);
        s_Sut2.Id.Should().Be(sut2.Id);
    }
}

public class ScoredPointTester
{

    [JsonPropertyName("id")]
    public OneOf<Guid, int> Id { get; set; } = default!;

}
