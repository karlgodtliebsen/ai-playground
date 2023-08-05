<<<<<<< HEAD
﻿using LLamaSharp.Domain.Configuration;

using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.Services;
=======
﻿using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.Services;
using LLamaSharpApp.WebAPI.Domain.Repositories;
using LLamaSharpApp.WebAPI.Domain.Repositories.Implementation;
using LLamaSharpApp.WebAPI.Domain.Services;
using LLamaSharpApp.WebAPI.Domain.Services.Implementations;
>>>>>>> main


using OptionsMapper = LLamaSharpApp.WebAPI.Controllers.Mappers.OptionsMapper;

namespace LLamaSharpApp.WebAPI.Configuration;

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
        services
            .AddLLamaDomain(configuration)
            .AddUtilities();
        return services;
    }


    private static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddScoped<IUserIdProvider, UserIdProvider>()
            .AddTransient<OptionsMapper>()
            .AddTransient<RequestMessagesMapper>()
            ;
        return services;
    }
}
