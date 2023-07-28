using ImageClassification.Domain.Trainers;
using ImageClassification.Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ImageClassification.Domain.Configuration;

public static class TensorFlowImageClassificationConfigurator
{
    public static IServiceCollection AddTensorFlowImageClassification(this IServiceCollection services,
        TensorFlowImageClassificationOptions options)
    {
        services.AddSingleton<IOptions<TensorFlowImageClassificationOptions>>(new OptionsWrapper<TensorFlowImageClassificationOptions>(options));
        services
        //    .AddTransient<IPredictor, Predictor>()
            .AddTransient<IKerasTrainer, KerasImageClassificationTrainer>()
            .AddTransient<ITensorFlowTrainer, ImageRecognitionInceptionTrainer>()
            .AddTransient<IImageLoader, ImageLoader>()
            ;
        return services;
    }

    public static IServiceCollection AddTensorFlowImageClassification(this IServiceCollection services, Action<TensorFlowImageClassificationOptions>? options = null)
    {
        var configuredOptions = new TensorFlowImageClassificationOptions();
        options?.Invoke(configuredOptions);
        return services.AddTensorFlowImageClassification(configuredOptions);
    }

    public static IServiceCollection AddTensorFlowImageClassification(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = TensorFlowImageClassificationOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<TensorFlowImageClassificationOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddTensorFlowImageClassification(configuredOptions);
    }
}
