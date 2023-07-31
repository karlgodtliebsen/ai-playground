using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace AI.Library.Utils;

[ExcludeFromCodeCoverage]
[Serializable]
public class HttpErrorException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public string Error { get; set; }
    public HttpErrorException()
    {
    }

    public HttpErrorException(string message) : base(message)
    {
    }

    public HttpErrorException(string message, HttpStatusCode statusCode) : base(message)
    {
        this.StatusCode = statusCode;
    }
    public HttpErrorException(HttpStatusCode statusCode)
    {
        this.StatusCode = statusCode;
    }

    public HttpErrorException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}