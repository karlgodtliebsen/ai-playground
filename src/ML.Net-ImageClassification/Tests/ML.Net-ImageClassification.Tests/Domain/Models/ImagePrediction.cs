using System.Globalization;

using Microsoft.ML.Data;

namespace ML.Net.ImageClassification.Tests.Domain.Models;

public class ImagePrediction
{
    [ColumnName("Score")]
    public float[] Score { get; set; } = Array.Empty<float>();

    public string Scores => string.Join(",", Score.Select(r => r.ToString(CultureInfo.InvariantCulture)));

    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; } = default!;
}
