using ImageClassification.Domain.Models;

namespace ML.TensorFlowApp.WebAPI.Domain.Services;

/// <summary>
/// Interface for Image Classification Service
/// </summary>
public interface IImageClassifierService
{
    /// <summary>
    /// Executes the Classification
    /// </summary>
    /// <param name="image"></param>
    /// <param name="dataSet"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> Classify(InMemoryImage image, string dataSet, CancellationToken cancellationToken);
}
