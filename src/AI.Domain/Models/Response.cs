namespace AI.Domain.Models;

public class Response<T>
{
    public Response(T value)
    {
        Value = value;
    }
    public T Value { get; }
}