namespace ImageClassification.Domain.Models;

public class InMemoryImage
{

    public InMemoryImage(byte[] image)
    {
        Image = image;
    }

    public byte[] Image { get; init; }
}

public class InMemoryImageData : InMemoryImage
{
    private readonly string[] fileNameParts;

    public InMemoryImageData(byte[] image, string label, string imageFileName, string fullName) : base(image)
    {
        Label = label;
        ImageFileName = imageFileName;
        FullName = fullName;
        fileNameParts = fullName.Split(Path.DirectorySeparatorChar);
    }
    public string[] FileNameParts()
    {
        return fileNameParts;
    }

    public string Label { get; set; }
    public string ImageFileName { get; init; }
    public string FullName { get; init; }
}
