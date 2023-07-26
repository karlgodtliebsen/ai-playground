namespace ML.Net.ImageClassification.Tests.Domain.Models;

public class InMemoryImageData
{
    public InMemoryImageData(byte[] image, string label, string imageFileName, string fullName)
    {
        Image = image;
        Label = label;
        ImageFileName = imageFileName;
        FullName = fullName;
    }

    public byte[] Image { get; init; }
    public string Label { get; init; }
    public string ImageFileName { get; init; }
    public string FullName { get; init; }
}
