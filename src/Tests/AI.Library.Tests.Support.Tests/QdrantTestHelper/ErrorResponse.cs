namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

public class ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }
    public string Error { get; init; }
}