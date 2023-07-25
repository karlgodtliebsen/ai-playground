using Microsoft.ML.Data;

namespace ML.Net.ImageClassification.Tests.Domain;

public class ImagePrediction
{
    [ColumnName("Score")]
    public float[] Score;

    public string Scores => string.Join(",", Score.Select(r => r.ToString()));

    [ColumnName("PredictedLabel")]
    public string PredictedLabel;
}
