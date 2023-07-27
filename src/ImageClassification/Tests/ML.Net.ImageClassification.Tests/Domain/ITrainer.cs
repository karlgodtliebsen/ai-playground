namespace ML.Net.ImageClassification.Tests.Domain;

public interface ITrainer
{
    string TrainModel(string imageSetPath, ImageLabelMapper? mapper);
}
