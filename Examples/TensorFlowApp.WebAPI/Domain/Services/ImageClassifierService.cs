using ImageClassification.Domain.Models;
using ImageClassification.Domain.Predictors;

namespace TensorFlowApp.WebAPI.Domain.Services;

/// <summary>
/// Chat Domain Service
/// </summary>
public class ImageClassifierService : IImageClassifierService
{
    private readonly IPredictor predictor;
    private readonly ILogger logger;


    /// <summary>
    /// Constructor for Image Classifier Service
    /// </summary>
    /// <param name="predictor"></param>
    /// <param name="logger"></param>
    public ImageClassifierService(IPredictor predictor, ILogger logger)
    {
        this.predictor = predictor;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task<string> Classify(InMemoryImage image, string dataSet, CancellationToken cancellationToken)
    {
        var result = predictor.PredictImage(image, dataSet);
        return Task.FromResult(result);
    }
}
