namespace ML.Net.ImageClassification.Tests.Domain.Models;

public class InMemoryImageData
{
    private readonly string[] fileNameParts;

    public InMemoryImageData(byte[] image, string label, string imageFileName, string fullName)
    {
        Image = image;
        Label = label;
        ImageFileName = imageFileName;
        FullName = fullName;
        this.fileNameParts = fullName.Split(Path.DirectorySeparatorChar);
    }
    public string[] FileNameParts()
    {
        return fileNameParts;
    }

    public byte[] Image { get; init; }
    public string Label { get; set; }
    public string ImageFileName { get; init; }
    public string FullName { get; init; }
}
