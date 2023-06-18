﻿using AI.Domain.Configuration;
using AI.Domain.Models;

using Microsoft.Extensions.Options;

namespace AI.Domain.AIClients;

public class CompletionAIClient : AIClientBase, ICompletionAIClient
{

    public CompletionAIClient(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger) : base(httpClientFactory, httpClient, options, logger)
    {
    }

    public async Task<Response<Completions>?> GetCompletionsAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        var result = await PostAsync<CompletionRequest, Completions>("completions", request, cancellationToken);
        return new Response<Completions>(result!);
    }





}