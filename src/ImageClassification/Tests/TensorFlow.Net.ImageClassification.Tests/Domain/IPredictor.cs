namespace ML.Net.ImageClassification.Tests.Domain;

public interface IPredictor
{
    void PredictImages(string imageSetPath, ImageLabelMapper? mapper);
}
