using ImageClassification.Domain.Models;

namespace ImageClassification.Domain.Predictors;

public interface IPredictor
{
    void PredictImages(string imageSetPath, ImageLabelMapper? mapper);
    string PredictImage(InMemoryImage image, string imageSetPath);
}
