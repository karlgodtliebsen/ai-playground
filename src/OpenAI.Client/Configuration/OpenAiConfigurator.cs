using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Client.AIClients;
using OpenAI.Client.AIClients.Implementation;
using OpenAI.Client.HttpUtils;
using Serilog;

namespace OpenAI.Client.Configuration;

public static class OpenAiConfigurator
{
    public static IServiceCollection AddAOpenAIConfiguration(this IServiceCollection services, OpenAIOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        services.AddHttpClient<ICompletionAIClient, CompletionAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());
        services.AddHttpClient<IChatCompletionAIClient, ChatCompletionAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IEmbeddingsAIClient, EmbeddingsAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IImagesAIClient, ImagesAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IFilesAIClient, FilesAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IEditsAIClient, EditsAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IModelsAIClient, ModelsAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        Log.Logger.Information("Completed Adding OpenAI Configuration to Application");

        return services;
    }

    public static IServiceCollection AddAOpenAIConfiguration(this IServiceCollection services, Action<OpenAIOptions>? options = null)
    {
        var configuredOptions = new OpenAIOptions();
        options?.Invoke(configuredOptions);
        return services.AddAOpenAIConfiguration(configuredOptions);
    }

    public static IServiceCollection AddAOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = OpenAIOptions.ConfigSectionName;
        }
        services.Configure<OpenAIOptions>(configuration.GetSection(sectionName));
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddAOpenAIConfiguration(configuredOptions);
    }

    public static IServiceCollection AddAzureAOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Starting Adding Azure OpenAI Configuration to Application");

        //services.Configure<OpenAIOptions>(configuration.GetSection(OpenAIOptions.ConfigSectionName));
        //services.AddHttpClient<IAIClient, AIClient>((sp, client) =>
        //    {
        //        var options = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        //        client.BaseAddress = options.GetBaseAddress();
        //    })
        //    .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
        //    .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());
        Log.Logger.Information("Completed Adding Azure OpenAI Configuration to Application");
        return services;
    }

}