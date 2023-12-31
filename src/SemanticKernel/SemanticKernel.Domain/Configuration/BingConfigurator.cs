﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SemanticKernel.Domain.Configuration;

public static class BingConfigurator
{
    public static IServiceCollection AddBing(this IServiceCollection services, BingOptions options)
    {
        services.AddSingleton(Options.Create(options));
        return services;
    }

    public static IServiceCollection AddBing(this IServiceCollection services, Action<BingOptions>? options = null)
    {
        var configuredOptions = new BingOptions();
        options?.Invoke(configuredOptions);
        return services.AddBing(configuredOptions);
    }

    public static IServiceCollection AddBing(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = BingOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<BingOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddBing(configuredOptions);
    }
}
