using System.Net;
using Microsoft.AspNetCore.Http;

namespace AI.Library.Utils;

public class ExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger logger)
    {
        this.logger = logger;
        this.next = next;
    }

    // Called by runtime for each request
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (HttpErrorException apiFriendlyException)
        {
            logger.Error(apiFriendlyException.ToString());
            await HandleApiException(httpContext, apiFriendlyException);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Unhandled exception caught by middleware");
            await HandleExceptionAsync(httpContext);
        }
    }

    private static Task HandleApiException(HttpContext context, HttpErrorException apiException)
    {
        return HandleException(context, apiException.StatusCode, apiException.Error);
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        return HandleException(context, HttpStatusCode.InternalServerError, "An internal error occurred");
    }

    private static Task HandleException(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(new ErrorDetails { StatusCode = context.Response.StatusCode, Message = message }.ToString());
    }
}