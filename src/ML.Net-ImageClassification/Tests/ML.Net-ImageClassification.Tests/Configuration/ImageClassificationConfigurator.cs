using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Domain;

namespace ML.Net.ImageClassification.Tests.Configuration;

public static class ImageClassificationConfigurator
{
    public static IServiceCollection AddImageClassification(this IServiceCollection services, ImageClassificationOptions options)
    {
        services.AddSingleton<IOptions<ImageClassificationOptions>>(new OptionsWrapper<ImageClassificationOptions>(options));

        services
            .AddTransient<IPredictor, Predictor>()
            .AddTransient<ITrainer, Trainer>()
            .AddTransient<IImageLoader, ImageLoader>()
            .AddTransient<IModelEvaluator, ModelEvaluator>()
            ;
        return services;
    }

    public static IServiceCollection AddImageClassification(this IServiceCollection services, Action<ImageClassificationOptions>? options = null)
    {
        var configuredOptions = new ImageClassificationOptions();
        options?.Invoke(configuredOptions);
        return services.AddImageClassification(configuredOptions);
    }

    public static IServiceCollection AddImageClassification(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = ImageClassificationOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<ImageClassificationOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddImageClassification(configuredOptions);
    }
}
