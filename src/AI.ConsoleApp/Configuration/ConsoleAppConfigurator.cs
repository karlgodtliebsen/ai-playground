﻿using AI.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AI.ConsoleApp.Configuration;

/// <summary>
/// 
/// </summary>
public static class ConsoleAppConfigurator
{
    public static HostApplicationBuilder AddSecrets<T>(this HostApplicationBuilder builder) where T : class
    {
        builder.Configuration.AddUserSecrets<T>();
        return builder;
    }

    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddAOpenAIConfiguration(configuration);
    }

}