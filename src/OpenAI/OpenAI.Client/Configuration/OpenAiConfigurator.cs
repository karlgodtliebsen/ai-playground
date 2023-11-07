using AI.Library.HttpUtils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.HttpClients.Implementation;

using Serilog;

namespace OpenAI.Client.Configuration;

public static class OpenAIConfigurator
{
    /// <summary>
    /// AddOpenAIConfiguration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, OpenAIConfiguration configuration)
    {
        Log.Logger.Information("Setting up OpenAI Configuration to Applications Configuration");

        ArgumentNullException.ThrowIfNull(configuration);
        services.AddSingleton(Options.Create(new OpenAiModelsVerification()));
        services.AddSingleton(Options.Create(configuration));

        services.AddTransient<IModelRequestFactory, ModelRequestFactory>();
        services.AddTransient<IOpenAiChatCompletionService, OpenAiChatCompletionService>();

        SetupHttpClients(services, configuration);

        Log.Logger.Information("Completed Adding OpenAI Configuration to Applications Configuration");
        return services;
    }

    private static void SetupHttpClients(IServiceCollection services, OpenAIConfiguration configuration)
    {
        services.AddHttpClient<ICompletionAIClient, CompletionAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IModerationAIClient, ModerationAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IChatCompletionAIClient, ChatCompletionAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IAudioFileAIClient, AudioFileAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IEmbeddingsAIClient, EmbeddingsAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IImagesAIClient, ImagesAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IFilesAIClient, FilesAIClient>((sp, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IEditsAIClient, EditsAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IModelsAIClient, ModelsAIClient>((_, client) =>
            {
                client.BaseAddress = configuration.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());
    }

    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, Action<OpenAIConfiguration>? options = null)
    {
        var configuredOptions = new OpenAIConfiguration();
        options?.Invoke(configuredOptions);
        return services.AddOpenAIConfiguration(configuredOptions);
    }

    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = OpenAIConfiguration.SectionName;
        }
        services.Configure<OpenAIConfiguration>(configuration.GetSection(sectionName));
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenAIConfiguration>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddOpenAIConfiguration(configuredOptions);
    }
}
