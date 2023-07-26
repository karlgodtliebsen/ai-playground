namespace ML.Net.ImageClassification.Tests.Domain.Models;

public sealed class ImageData
{
    private readonly string fullFileName;
    private readonly string[] fileNameParts;

    public ImageData(string fullFileName, string label, string imagePath)
    {
        this.fullFileName = fullFileName;
        Label = label;
        this.fileNameParts = fullFileName.Split(Path.DirectorySeparatorChar);
        ImagePath = imagePath;
    }
    //To avoid this being part of the Schema, it needs to be a Method
    public string FullFileName()
    {
        return fullFileName;
    }
    //To avoid this being part of the Schema, it needs to be a Method
    public string[] FileNameParts()
    {
        return fileNameParts;
    }


    public string Label { get; set; }

    public string ImagePath { get; }
}
