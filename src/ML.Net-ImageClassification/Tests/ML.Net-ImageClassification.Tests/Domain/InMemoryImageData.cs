namespace ML.Net.ImageClassification.Tests.Domain;

public class InMemoryImageData
{
    public InMemoryImageData(byte[] image, string label, string imageFileName, string fullName)
    {
        Image = image;
        Label = label;
        ImageFileName = imageFileName;
        FullName = fullName;
    }

    public readonly byte[] Image;
    public readonly string Label;
    public readonly string ImageFileName;
    public readonly string FullName;

}
