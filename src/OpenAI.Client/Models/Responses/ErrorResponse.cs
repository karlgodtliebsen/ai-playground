namespace OpenAI.Client.Models.Responses;

//public class Response<T>
//{
//    public bool Success { get; set; } = false;

//    public Response(T value)
//    {
//        Value = value;
//        if (value is not null)
//        {
//            Success = true;
//        }
//    }
//    public T Value { get; }
//}



public class ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }
    public string Error { get; init; }
}
