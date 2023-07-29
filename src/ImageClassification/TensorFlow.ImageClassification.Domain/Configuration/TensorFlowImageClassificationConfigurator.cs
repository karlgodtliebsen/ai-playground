using ImageClassification.Domain.Predictors;
using ImageClassification.Domain.Trainers;
using ImageClassification.Domain.TransferLearning;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ImageClassification.Domain.Configuration;

public static class TensorFlowImageClassificationConfigurator
{
    public static IServiceCollection AddTensorFlowImageClassification(this IServiceCollection services,
        TensorFlowOptions options)
    {
        services.AddSingleton<IOptions<TensorFlowOptions>>(new OptionsWrapper<TensorFlowOptions>(options));
        services
            .AddTransient<IKerasTrainer, KerasImageClassificationTrainer>()
            .AddTransient<ITensorFlowTrainer, TensorFlowInceptionTrainer>()
            .AddTransient<ITensorFlowTransferLearningInception, TensorFlowTransferLearningInception>()
            .AddTransient<IPredictor, TensorFlowTransferLearningInceptionPredictor>()
            .AddTransient<ITester, TensorFlowTransferLearningInceptionTester>()
            .AddTransient<IImageLoader, ImageLoader>()
            .AddTransient<ExtendedModelFactory>()
            .AddTransient<ExtendedTransferLearning>()
            .AddSingleton<IOptions<ExtendedTaskOptions>>(new OptionsWrapper<ExtendedTaskOptions>(new ExtendedTaskOptions()))
            ;
        return services;
    }

    public static IServiceCollection AddTensorFlowImageClassification(this IServiceCollection services, Action<TensorFlowOptions>? options = null)
    {
        var configuredOptions = new TensorFlowOptions();
        options?.Invoke(configuredOptions);
        return services.AddTensorFlowImageClassification(configuredOptions);
    }

    public static IServiceCollection AddTensorFlowImageClassification(this IServiceCollection services, IConfiguration configuration, string? sectionName = null)
    {
        if (sectionName is null)
        {
            sectionName = TensorFlowOptions.SectionName;
        }
        var configuredOptions = configuration.GetSection(sectionName).Get<TensorFlowOptions>()!;
        ArgumentNullException.ThrowIfNull(configuredOptions);
        return services.AddTensorFlowImageClassification(configuredOptions);
    }
}
