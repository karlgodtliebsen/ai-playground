﻿using AI.Library.HttpUtils;

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
    public static IServiceCollection AddOpenAIConfiguration(this IServiceCollection services, OpenAIOptions options)
    {
        services.AddTransient<IModelRequestFactory, ModelRequestFactory>();
        services.AddSingleton<IOptions<OpenAiModelsVerification>>(new OptionsWrapper<OpenAiModelsVerification>(new OpenAiModelsVerification()));

        ArgumentNullException.ThrowIfNull(options);

        services.AddHttpClient<ICompletionAIClient, CompletionAIClient>((sp, client) =>
        {
            client.BaseAddress = options.GetBaseAddress();
        })
            .AddPolicyHandler(HttpClientsPolicies.GetCircuitBreakerPolicyForNotFound())
            .AddPolicyHandler(HttpClientsPolicies.GetRetryPolicy());

        services.AddHttpClient<IModerationAIClient, ModerationAIClient>((sp, client) =>
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

        services.AddHttpClient<IAudioFileAIClient, AudioFileAIClient>((sp, client) =>
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
        Log.Logger.Information("Starting Adding Azure OpenAI Configuration to Application");

        if (sectionName is null)
        {
            sectionName = OpenAIOptions.ConfigSectionName;
        }
        services.Configure<OpenAIOptions>(configuration.GetSection(sectionName));
        var configuredOptions = configuration.GetSection(sectionName).Get<OpenAIOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        //Bind directly to the options when validation is not required
        /// var section = configuration.GetSection(sectionName);
        ///services.AddOptions<OpenAIOptions>().Bind(section);

        Log.Logger.Information("Completed Adding Azure OpenAI Configuration to Application");
        return services.AddOpenAIConfiguration(configuredOptions);
    }


}
