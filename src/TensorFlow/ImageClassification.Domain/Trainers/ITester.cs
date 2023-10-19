using ImageClassification.Domain.Models;

namespace ImageClassification.Domain.Trainers;

public interface ITester
{
    float TestModel(string imageSetPath, ImageLabelMapper? mapper);
}
