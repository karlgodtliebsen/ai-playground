using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients.Implementation;

public class ModerationAIClient : AIClientBase, IModerationAIClient
{

    public ModerationAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<Response<ModerationResponse>?> GetModerationAsync(ModerationRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<ModerationRequest, ModerationResponse>(request.RequestUri, request, cancellationToken);
        return new Response<ModerationResponse>(result!);
    }
}