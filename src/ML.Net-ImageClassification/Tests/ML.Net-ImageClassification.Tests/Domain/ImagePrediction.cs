using Microsoft.ML.Data;

namespace ML.Net.ImageClassification.Tests.Domain;

public class ImagePrediction
{
    [ColumnName("Score")]
    public float[] Score;

    [ColumnName("PredictedLabel")]
    public string PredictedLabel;
}
