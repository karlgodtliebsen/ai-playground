using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace OpenAI.Client.HttpUtils;

public static class HttpClientsPolicies
{

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicyForNotFound()
    {
        return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.NotFound)
                .CircuitBreakerAsync(1, TimeSpan.FromMicroseconds(1))
            ;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}