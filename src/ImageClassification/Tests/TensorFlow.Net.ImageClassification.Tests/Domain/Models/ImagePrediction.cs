using System.Globalization;

namespace ML.Net.ImageClassification.Tests.Domain.Models;

public class ImagePrediction
{
    public float[] Score { get; set; } = Array.Empty<float>();

    public string Scores => string.Join(",", Score.Select(r => r.ToString(CultureInfo.InvariantCulture)));

    //[ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; } = default!;
}
