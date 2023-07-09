namespace AI.VectorDatabase.Qdrant.VectorStorage;

public class ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }
    public string Error { get; init; }
}
