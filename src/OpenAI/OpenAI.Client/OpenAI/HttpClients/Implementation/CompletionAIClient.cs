﻿using System.Runtime.CompilerServices;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

namespace OpenAI.Client.OpenAI.HttpClients.Implementation;

public class CompletionAIClient : AIClientBase, ICompletionAIClient
{
    public CompletionAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIConfiguration> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<OneOf<Completions, ErrorResponse>> GetCompletionsAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<CompletionRequest, Completions>(request.RequestUri, request, cancellationToken);
        return result;
    }

    public async IAsyncEnumerable<OneOf<Completions, ErrorResponse>> GetCompletionsStreamAsync(CompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var collection = GetResponseStreamAsync<Completions, CompletionRequest>(request.RequestUri, request, cancellationToken);
        await foreach (var result in collection.WithCancellation(cancellationToken))
        {
            yield return result;
        }
    }
}
