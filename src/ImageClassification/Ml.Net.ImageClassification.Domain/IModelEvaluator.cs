using Microsoft.ML;

namespace ImageClassification.Domain;

public interface IModelEvaluator
{
    void EvaluateModel(MLContext mlContext, IDataView testDataset, ITransformer trainedModel);
    //void TrySinglePrediction(string imagesFolderPathForPredictions, MLContext mlContext, ITransformer trainedModel, IImageLoader imageLoader);
    //void WriteImagePrediction(string imagePath, string label, string predictedLabel, float probability);
}
