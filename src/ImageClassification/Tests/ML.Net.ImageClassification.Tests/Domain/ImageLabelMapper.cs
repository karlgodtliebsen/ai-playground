namespace ML.Net.ImageClassification.Tests.Domain;

public class ImageLabelMapper
{
    public ImageLabelMapper(string fileName, int imageIndex, int? labelIndex = default, (int from, int to)? labelIndexRange = default)
    {
        LabelIndex = labelIndex;
        FileName = fileName;
        ImageIndex = imageIndex;
        LabelIndexRange = labelIndexRange;
    }

    public int? LabelIndex { get; init; }
    public (int from, int to)? LabelIndexRange { get; init; }

    public string FileName { get; init; }
    public int ImageIndex { get; init; }
}
