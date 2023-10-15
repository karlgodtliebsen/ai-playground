using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticMemory;
using Microsoft.SemanticMemory.WebService;


namespace Semantic.Memory.WebAPI.Configuration;

/// <summary>
/// WebApi Configuration 
/// </summary>
public static class WebApiConfigurator
{
    /// <summary>
    /// Add WebAPI options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Inject memory client and its dependencies
        // Note: pass the current service collection to the builder, in order to start the pipeline handlers
        ISemanticMemoryClient memory = new MemoryClientBuilder(services).FromAppSettings().Build();
        services.AddSingleton(memory);

        services
            //.AddDomain(configuration)
            .AddUtilities();
        return services;
    }


    private static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
        //    .AddScoped<IUserIdProvider, UserIdProvider>()
        //    .AddTransient<OptionsMapper>()
        //    .AddTransient<RequestMessagesMapper>()
            ;
        return services;
    }


    public static WebApplication MapEndpoints(this WebApplication app)
    {
        DateTimeOffset start = DateTimeOffset.UtcNow;

        // Simple ping endpoint
        app.MapGet("/", () => Results.Ok($"Ingestion service is running. Uptime: {(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - start.ToUnixTimeSeconds())} secs - Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}"));


        // File upload endpoint
        app.MapPost(Constants.HttpUploadEndpoint, async Task<IResult> (
                HttpRequest request,
                ISemanticMemoryClient service,
                ILogger<Program> log,
                CancellationToken cancellationToken) =>
        {
            log.LogTrace("New upload HTTP request");

            // Note: .NET doesn't yet support binding multipart forms including data and files
            (HttpDocumentUploadRequest input, bool isValid, string errMsg)
                = await HttpDocumentUploadRequest.BindHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

            if (!isValid)
            {
                log.LogError(errMsg);
                return Results.BadRequest(errMsg);
            }

            try
            {
                // UploadRequest => Document
                var documentId = await service.ImportDocumentAsync(input.ToDocumentUploadRequest(), cancellationToken);
                var url = Constants.HttpUploadStatusEndpointWithParams
                    .Replace(Constants.HttpIndexPlaceholder, input.Index, StringComparison.Ordinal)
                    .Replace(Constants.HttpDocumentIdPlaceholder, documentId, StringComparison.Ordinal);
                return Results.Accepted(url, new UploadAccepted
                {
                    DocumentId = documentId,
                    Index = input.Index,
                    Message = "Document upload completed, ingestion pipeline started"
                });
            }
            catch (Exception e)
            {
                return Results.Problem(title: "Document upload failed", detail: e.Message, statusCode: 503);
            }
        })
            .Produces<UploadAccepted>(StatusCodes.Status202Accepted);

        // Delete document endpoint
        app.MapDelete(Constants.HttpDocumentsEndpoint,
                async Task<IResult> (
                    [FromQuery(Name = Constants.WebServiceIndexField)]
                string? index,
                    [FromQuery(Name = Constants.WebServiceDocumentIdField)]
                string documentId,
                    ISemanticMemoryClient service,
                    ILogger<Program> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New delete document HTTP request");
                    await service.DeleteDocumentAsync(documentId: documentId, index: index, cancellationToken);
                    return Results.Accepted();
                })
            .Produces<MemoryAnswer>(StatusCodes.Status202Accepted);

        // Ask endpoint
        app.MapPost(Constants.HttpAskEndpoint,
                async Task<IResult> (
                    MemoryQuery query,
                    ISemanticMemoryClient service,
                    ILogger<Program> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New search request");
                    MemoryAnswer answer = await service.AskAsync(question: query.Question, index: query.Index, filters: query.Filters, cancellationToken: cancellationToken);
                    return Results.Ok(answer);
                })
            .Produces<MemoryAnswer>(StatusCodes.Status200OK);

        // Search endpoint
        app.MapPost(Constants.HttpSearchEndpoint,
                async Task<IResult> (
                    SearchQuery query,
                    ISemanticMemoryClient service,
                    ILogger<Program> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New search HTTP request");
                    SearchResult answer = await service.SearchAsync(query: query.Query, index: query.Index, limit: query.Limit, filters: query.Filters, cancellationToken: cancellationToken);
                    return Results.Ok(answer);
                })
            .Produces<SearchResult>(StatusCodes.Status200OK);

        // Document status endpoint
        app.MapGet(Constants.HttpUploadStatusEndpoint,
                async Task<IResult> (
                    [FromQuery(Name = Constants.WebServiceIndexField)]
                string? index,
                    [FromQuery(Name = Constants.WebServiceDocumentIdField)]
                string documentId,
                    ISemanticMemoryClient memoryClient,
                    ILogger<Program> log,
                    CancellationToken cancellationToken) =>
                {
                    log.LogTrace("New document status HTTP request");
                    index = IndexExtensions.CleanName(index);

                    if (string.IsNullOrEmpty(documentId))
                    {
                        return Results.BadRequest($"'{Constants.WebServiceDocumentIdField}' query parameter is missing or has no value");
                    }

                    DataPipelineStatus? pipeline = await memoryClient.GetDocumentStatusAsync(documentId: documentId, index: index, cancellationToken);
                    if (pipeline == null)
                    {
                        return Results.NotFound("Document not found");
                    }

                    if (pipeline.Empty)
                    {
                        return Results.NotFound(pipeline);
                    }

                    return Results.Ok(pipeline);
                })
            .Produces<MemoryAnswer>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}
