using AI.Test.Support.Fixtures;

using LlamaSharp.Tests.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LlamaSharp.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IntegrationTestWebApplicationFactory : IntegrationTestWebApplicationFactory<Program>
{

    protected override void ConfigureTestServices(IServiceCollection services, IConfiguration cfg)
    {
        const string endpointUrl = "https://localhost";
        var options = new LlamaClientOptions()
        {
            Endpoint = endpointUrl,
        };
        services.AddSingleton<IOptions<LlamaClientOptions>>(new OptionsWrapper<LlamaClientOptions>(options));
        LLamaConfigurationClient ConfigClient(HttpClient c, IServiceProvider sp)
        {
            var claimsProvider = sp.GetRequiredService<TestClaimsProvider>();
            var client = this.CreateClientWithTestAuth(claimsProvider);
            return new LLamaConfigurationClient(client, sp.GetRequiredService<IOptions<LlamaClientOptions>>(), sp.GetRequiredService<ILogger>());
        }

        services.AddHttpClient<ILLamaConfigurationClient, LLamaConfigurationClient>(ConfigClient)
            .AddPolicyHandler(GetCircuitBreakerPolicyForCustomerServiceNotFound())
            ;

        LLamaCompositeOperationsClient CompositeClient(HttpClient c, IServiceProvider sp)
        {
            var claimsProvider = sp.GetRequiredService<TestClaimsProvider>();
            var client = this.CreateClientWithTestAuth(claimsProvider);
            return new LLamaCompositeOperationsClient(client, sp.GetRequiredService<IOptions<LlamaClientOptions>>(), sp.GetRequiredService<ILogger>());
        }

        services.AddHttpClient<ILLamaCompositeOperationsClient, LLamaCompositeOperationsClient>(CompositeClient)
            .AddPolicyHandler(GetCircuitBreakerPolicyForCustomerServiceNotFound())
            ;
    }

}

