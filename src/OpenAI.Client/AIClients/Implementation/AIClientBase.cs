using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Responses;

using SerilogTimings.Extensions;

namespace OpenAI.Client.AIClients.Implementation;

public abstract class AIClientBase
{
    private const string UserAgent = "ai/openai_api";
    protected readonly HttpClient HttpClient;
    protected JsonSerializerOptions SerializerOptions;
    protected readonly OpenAIOptions Options;
    protected readonly ILogger logger;

    protected AIClientBase(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger)
    {
        HttpClient = httpClient;
        this.logger = logger;
        this.Options = options.Value;
        SerializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    protected async Task<OneOf<TR, ErrorResponse>> PostAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("PostAsync", subUri);
        try
        {
            PrepareClient();

            var response = await HttpClient.PostAsJsonAsync(subUri, payload, SerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TR>(cancellationToken: cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "PostAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    protected async Task<OneOf<TR, ErrorResponse>> GetAsync<TR>(string subUri, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("GetAsync", subUri);
        try
        {
            PrepareClient();
            var result = await HttpClient.GetFromJsonAsync<TR>(subUri, cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    protected async Task<OneOf<string, ErrorResponse>> GetContentAsync(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("GetContentAsync", subUri);
        try
        {
            PrepareClient();
            var result = await HttpClient.GetStringAsync(subUri, cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetContentAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    protected async Task<OneOf<TR, ErrorResponse>> DeleteAsync<TR>(string subUri, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("DeleteAsync", subUri);
        try
        {
            PrepareClient();
            var result = await HttpClient.DeleteFromJsonAsync<TR>(subUri, cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "DeleteAsync Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }

    }


    protected async Task<OneOf<string, ErrorResponse>> PostAsyncWithStream<T, TR>(string subUri, T payload, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("PostAsyncWithStream", subUri);
        try
        {
            PrepareClient();

            var response = await HttpClient.PostAsJsonAsync(subUri, payload, SerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
            op.Complete();
            return result!;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "PostAsyncWithStream Failed {uri}", subUri);
            return new ErrorResponse(ex.Message);
        }
    }

    /// <summary>
    /// TS:  ResponseStream<TR>
    /// TR : Request
    /// T : Request object
    /// </summary>
    /// <typeparam name="TS"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="subUri"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OneOf<TS, ErrorResponse>> GetResponseUsingStreamAsync<TS, TR, T>(string subUri, T request, CancellationToken cancellationToken)
        where TS : ResponseStream<TR>, new()
        where TR : class
    {

        var result = await PostAsyncWithStream<T, TS>(subUri, request, cancellationToken);
        return result.Match<OneOf<TS, ErrorResponse>>(
            success =>
            {
                var data = success.Split("data:");
                var resp = new TS();
                foreach (var v in data.Where(d => !string.IsNullOrEmpty(d) && !d.Contains("[DONE]")))
                {
                    var obj = JsonSerializer.Deserialize<TR>(v);
                    resp.Data.Add(obj!);
                }
                return resp;
            },
            error => new ErrorResponse(error.Error)
        );
    }

    protected void PrepareClient()
    {
        HttpClient.DefaultRequestHeaders.Clear();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Options.ApiKey);

        // Further authentication-header used for Azure openAI service
        HttpClient.DefaultRequestHeaders.Add("api-key", Options.ApiKey);
        HttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        HttpClient.DefaultRequestHeaders.Add("OpenAI-Organization", Options.OrganisationKey);
    }


}
