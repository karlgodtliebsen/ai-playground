using ImageClassification.Domain.Predictors;
using ImageClassification.Domain.Trainers;
using ImageClassification.Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ImageClassification.Domain.Configuration;

public static class MlNetImageClassificationConfigurator
{
    public static IServiceCollection AddMlnetImageClassification(this IServiceCollection services, MlImageClassificationOptions options)
    {
        services.AddSingleton<IOptions<MlImageClassificationOptions>>(new OptionsWrapper<MlImageClassificationOptions>(options));

        services
            .AddTransient<IPredictor, Predictor>()
            .AddTransient<IMlNetTrainer, MlNetTrainer>()
            .AddTransient<IImageLoader, ImageLoader>()
            .AddTransient<IModelEvaluator, ModelEvaluator>()
            ;
        return services;
    }

    public static IServiceCollection AddMlnetImageClassification(this IServiceCollection services, Action<MlImageClassificationOptions>? options = null)
    {
        var configuredOptions = new MlImageClassificationOptions();
        options?.Invoke(configuredOptions);
        return services.AddMlnetImageClassification(configuredOptions);
    }

    public static IServiceCollection AddMlnetImageClassification(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = MlImageClassificationOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<MlImageClassificationOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddMlnetImageClassification(configuredOptions);
    }
}