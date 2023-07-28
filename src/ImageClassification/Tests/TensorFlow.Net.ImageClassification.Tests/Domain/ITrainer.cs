using ML.Net.ImageClassification.Tests.Domain.Models;

namespace ML.Net.ImageClassification.Tests.Domain;

public interface ITrainer
{
    string TrainModel(string imageSetPath, ImageLabelMapper? mapper);
}


public interface ITensorFlowTrainer : ITrainer
{

}

public interface IKerasTrainer : ITrainer
{

}
