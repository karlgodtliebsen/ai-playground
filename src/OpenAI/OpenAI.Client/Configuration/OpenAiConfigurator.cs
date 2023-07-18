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
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, OpenAIOptions options)
    {
        Log.Logger.Information("Setting up OpenAI Configuration to Applications Configuration");

        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton<IOptions<OpenAiModelsVerification>>(new OptionsWrapper<OpenAiModelsVerification>(new OpenAiModelsVerification()));
        services.AddSingleton<IOptions<OpenAIOptions>>(new OptionsWrapper<OpenAIOptions>(options));

        services.AddTransient<IModelRequestFactory, ModelRequestFactory>();
        services.AddTransient<IOpenAiChatCompletionService, OpenAiChatCompletionService>();

        SetupHttpClients(services, options);

        Log.Logger.Information("Completed Adding OpenAI Configuration to Applications Configuration");
        return services;
    }

    private static void SetupHttpClients(IServiceCollection services, OpenAIOptions options)
    {
        services.AddHttpClient<ICompletionAIClient, CompletionAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IModerationAIClient, ModerationAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IChatCompletionAIClient, ChatCompletionAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IAudioFileAIClient, AudioFileAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IEmbeddingsAIClient, EmbeddingsAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IImagesAIClient, ImagesAIClient>((_, client) =>
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

        services.AddHttpClient<IEditsAIClient, EditsAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IModelsAIClient, ModelsAIClient>((_, client) =>
            {
                client.BaseAddress = options.GetBaseAddress();
            })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());
    }

    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, Action<OpenAIOptions>? options = null)
    {
        var configuredOptions = new OpenAIOptions();
        options?.Invoke(configuredOptions);
        return services.AddOpenAIConfiguration(configuredOptions);
    }

    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = OpenAIOptions.ConfigSectionName;
        }
        services.Configure<OpenAIOptions>(configuration.GetSection(sectionName));
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddOpenAIConfiguration(configuredOptions);
    }


    public static IServiceCollection AddAzureOpenAIConfiguration(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        Log.Logger.Information("Starting Adding Azure OpenAI Configuration to Applications Configuration");

        if (sectionName is null)
        {
            sectionName = AzureOpenAIOptions.ConfigSectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<AzureOpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        services.AddSingleton<IOptions<AzureOpenAIOptions>>(new OptionsWrapper<AzureOpenAIOptions>(configuredOptions));

        Log.Logger.Information("Completed Adding Azure OpenAI Configuration to Applications Configuration");
        return services;
    }


}
