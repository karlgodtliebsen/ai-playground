using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OpenAI.Client.Configuration;
using SerilogTimings.Extensions;

namespace OpenAI.Client.AIClients.Implementation;

public abstract class AIClientBase
{
    private const string UserAgent = "ai/openai_api";
    protected readonly HttpClient HttpClient;
    protected JsonSerializerOptions SerializerOptions;
    protected readonly OpenAIOptions options;
    protected readonly ILogger logger;

    protected AIClientBase(
        IHttpClientFactory httpClientFactory,
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger logger)
    {
        HttpClient = httpClient;
        this.logger = logger;
        this.options = options.Value;
        SerializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    protected async Task<TR?> PostAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("PostAsync", subUri);
        try
        {
            PrepareClient();

            var response = await HttpClient.PostAsJsonAsync(subUri, payload, SerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TR>(cancellationToken: cancellationToken);
            op.Complete();
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "PostAsync Failed {uri}", subUri);
        }

        return default;
    }

    protected async Task<TR?> GetAsync<TR>(string subUri, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("GetAsync", subUri);
        try
        {
            PrepareClient();
            var result = await HttpClient.GetFromJsonAsync<TR>(subUri, cancellationToken);
            op.Complete();
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync Failed {uri}", subUri);
        }

        return default;
    }

    protected async Task<string> GetContentAsync(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation("GetContentAsync", subUri);
        try
        {
            PrepareClient();
            var result = await HttpClient.GetStringAsync(subUri, cancellationToken);
            op.Complete();
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetContentAsync Failed {uri}", subUri);
        }

        return default;
    }
    protected async Task<TR?> DeleteAsync<TR>(string subUri, CancellationToken cancellationToken) where TR : class
    {
        using var op = logger.BeginOperation("DeleteAsync", subUri);
        try
        {
            PrepareClient();
            var result = await HttpClient.DeleteFromJsonAsync<TR>(subUri, cancellationToken);
            op.Complete();
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "DeleteAsync Failed {uri}", subUri);
        }

        return default;
    }
    protected void PrepareClient()
    {
        HttpClient.DefaultRequestHeaders.Clear();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);

        // Further authentication-header used for Azure openAI service
        HttpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
        HttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        HttpClient.DefaultRequestHeaders.Add("OpenAI-Organization", options.OrganisationKey);
    }


}