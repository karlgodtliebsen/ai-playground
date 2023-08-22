using System.Text.Json.Serialization;

namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

public class HttpStatusResponse
{
    [JsonPropertyName("status")]
    public HttpErrorResponse? Status { get; init; }
    [JsonPropertyName("time")]
    public double Time { get; init; }
}