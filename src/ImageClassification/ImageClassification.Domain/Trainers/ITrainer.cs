using ImageClassification.Domain.Models;

namespace ImageClassification.Domain.Trainers;

public interface ITrainer
{
    string TrainModel(string imageSetPath, ImageLabelMapper? mapper);
}
