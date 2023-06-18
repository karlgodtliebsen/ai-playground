namespace AI.Domain.Models.Responses;

public class Response<T>
{
    public bool Success { get; set; } = false;

    public Response(T value)
    {
        Value = value;
        if (value is not null)
        {
            Success = true;
        }
    }
    public T Value { get; }
}