using LLamaSharp.Domain.Domain.Services;
using LLamaSharp.Domain.Domain.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LLamaSharp.Domain.Configuration;




/// <summary>
/// LLama Domain Configuration 
/// </summary>
public static class LLamaDomainConfigurator
{
    /// <summary>
    /// Add LLama options
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLLamaDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddLlamaConfiguration(configuration)
            .AddInferenceConfiguration(configuration)
            .AddLLamaRepository(configuration);

        services
            .AddTransient<ILlamaModelFactory, LlamaModelFactory>()
            .AddTransient<IOptionsService, OptionsService>()
            .AddTransient<IChatDomainService, ChatDomainService>()
            .AddTransient<IEmbeddingsService, EmbeddingsService>()
            .AddTransient<IExecutorService, ExecutorService>()
            .AddTransient<ITokenizationService, TokenizationService>()
            ;

        return services;
    }
}
