namespace ML.Net.ImageClassification.Tests.Domain;

public class ImageLabelMapper
{
    public ImageLabelMapper(int imageIndex, int labelIndex, string fileName)
    {
        LabelIndex = labelIndex;
        FileName = fileName;
        ImageIndex = imageIndex;
    }

    public int LabelIndex { get; init; }
    public string FileName { get; init; }
    public int ImageIndex { get; init; }
}
