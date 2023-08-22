using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

public class HttpErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}