namespace OpenAI.Client.Models.Responses;

public class ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }
    public string Error { get; init; }
}
