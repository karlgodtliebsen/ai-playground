namespace AI.Library.HttpUtils;

public class ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }
    public string Error { get; init; }
}
