using ImageClassification.Domain.Models;

namespace ImageClassification.Domain.Predictors;

public interface IPredictor
{
    void PredictImages(string imageSetPath, ImageLabelMapper? mapper);
}
