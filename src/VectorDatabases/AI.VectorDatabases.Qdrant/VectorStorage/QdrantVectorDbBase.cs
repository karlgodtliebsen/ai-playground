using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using AI.VectorDatabase.Qdrant.Configuration;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using OneOf;

using Serilog;

using SerilogTimings;
using SerilogTimings.Extensions;

namespace AI.VectorDatabase.Qdrant.VectorStorage;

public abstract class QdrantVectorDbBase
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    protected readonly JsonSerializerOptions serializerOptions;
    private readonly QdrantOptions options;

    protected QdrantVectorDbBase(HttpClient httpClient, QdrantOptions options, ILogger logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.options = options;
        serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private async Task<OneOf<TR, ErrorResponse>> VerifyResult<TR>(HttpResponseMessage response, string subUri, Operation op, CancellationToken cancellationToken)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
        {
            var result = await response.Content.ReadFromJsonAsync<QdrantHttpResponse<TR>>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return await HandleError(response, subUri, cancellationToken);
            }
            if (result.Status is not OperationStatus.Succeeded)
            {
                return await HandleError(response, subUri, cancellationToken);
            }
            op.Complete();
            return result!.Result;
        }
        return await HandleError(response, subUri, cancellationToken);
    }

    private OneOf<TR, ErrorResponse> VerifyResult<TR>(QdrantHttpResponse<TR>? response, Operation op)
    {
        if (response is null)
        {
            return new ErrorResponse($"Operation failed.");
        }

        if (response.Status is not OperationStatus.Succeeded)
        {
            return new ErrorResponse($"Operation failed with status {response.Status}.");
        }

        op.Complete();
        return response!.Result;
    }

    private ErrorResponse HandleError(Exception ex, string subUri)
    {
        logger.Error(ex, "Operation Failed for {uri}", subUri);
        return new ErrorResponse($"Operation Failed for {subUri} {ex.Message}");
    }

    private async Task<ErrorResponse> HandleError(HttpResponseMessage? response, string subUri, CancellationToken cancellationToken, Exception? ex = null)
    {
        if (ex is not null)
        {
            logger.Error(ex, "Operation Failed {uri}", subUri);
            return new ErrorResponse("Fatal Error Calling Qdrant.\n" + ex.Message);
        }
        if (response is not null && response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var result = await response.Content.ReadFromJsonAsync<QdrantVectorDb.HttpStatusResponse>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return new ErrorResponse($"Operation failed.");
            }

            if (result.Status is not null)
            {
                return new ErrorResponse($"Operation failed with status: {result.Status.Error}.");
            }
        }
        return new ErrorResponse("Fatal Error Calling Qdrant");
    }


    protected async Task<OneOf<TR, ErrorResponse>> PostAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PostAsync {subUri}");
        HttpResponseMessage? response = null;
        try
        {
            PrepareClient();
            response = await httpClient.PostAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
            return await VerifyResult<TR>(response, subUri, op, cancellationToken);
        }
        catch (Exception ex)
        {
            return await HandleError(response, subUri, cancellationToken, ex);
        }
    }

    protected async Task<OneOf<TR, ErrorResponse>> PutAsync<T, TR>(string subUri, T payload, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PutAsync {subUri}");
        HttpResponseMessage? response = null;
        try
        {
            PrepareClient();
            response = await httpClient.PutAsJsonAsync(subUri, payload, serializerOptions, cancellationToken);
            return await VerifyResult<TR>(response, subUri, op, cancellationToken);
        }
        catch (Exception ex)
        {
            return await HandleError(response, subUri, cancellationToken, ex);
        }
    }
    protected async Task<OneOf<TR, ErrorResponse>> PutAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"PutAsync {subUri}");
        HttpResponseMessage? response = null;
        try
        {
            PrepareClient();
            response = await httpClient.PutAsJsonAsync(subUri, serializerOptions, cancellationToken);
            return await VerifyResult<TR>(response, subUri, op, cancellationToken);
        }
        catch (Exception ex)
        {
            return await HandleError(response, subUri, cancellationToken, ex);
        }
    }

    protected async Task<OneOf<TR, ErrorResponse>> GetAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"GetAsync {subUri}");
        try
        {
            PrepareClient();
            var response = await httpClient.GetFromJsonAsync<QdrantHttpResponse<TR>>(subUri, cancellationToken)!;
            return VerifyResult(response, op);
        }
        catch (Exception ex)
        {
            return HandleError(ex, subUri);
        }
    }

    protected async Task<OneOf<TR, ErrorResponse>> DeleteAsync<TR>(string subUri, CancellationToken cancellationToken)
    {
        using var op = logger.BeginOperation($"DeleteAsync {subUri}");
        try
        {
            PrepareClient();
            var response = await httpClient.DeleteFromJsonAsync<QdrantHttpResponse<TR>>(subUri, cancellationToken);
            return VerifyResult(response, op);
        }
        catch (Exception ex)
        {
            return HandleError(ex, subUri);
        }
    }

    private void PrepareClient()
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
    }
}
