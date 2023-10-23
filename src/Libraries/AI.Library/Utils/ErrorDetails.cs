namespace AI.Library.Utils;

public class ErrorDetails
{
    public required int StatusCode { get; set; }
    public required string Message { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}