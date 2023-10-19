namespace ImageClassification.Domain.Models;

public sealed class TrainingData
{
    private static readonly char[] Separators = new char[] { '/', '\\' };
    public TrainingData(string fileName, string label)
    {
        FileName = fileName;
        FileNameParts = fileName.Split(Separators);
        Label = label;
    }

    public string[] FileNameParts { get; set; }

    public string FileName { get; }
    public string Label { get; }

    public bool FindMatchingLabelForImage(ImageData imageData)
    {
        if (FileName == imageData.ImagePath)
            return true;
        if (FileNameParts.Length <= 1)
            return false;

        //0.0,train/ABBOTTS BABBLER/001.jpg,ABBOTTS BABBLER,train,MALACOCINCLA ABBOTTI
        //Image_1.jpg,SOUTHERN DOGFACE
        if (
            FileNameParts[^1] == imageData.FileNameParts()[^1]
            &&
            FileNameParts[^2] == imageData.FileNameParts()[^2]
            )
            return true;

        return false;
    }
    public bool FindMatchingLabelForImage(InMemoryImageData imageData)
    {
        if (FileName == imageData.ImageFileName)
            return true;
        if (FileNameParts.Length <= 1)
            return false;

        //0.0,train/ABBOTTS BABBLER/001.jpg,ABBOTTS BABBLER,train,MALACOCINCLA ABBOTTI
        //Image_1.jpg,SOUTHERN DOGFACE
        if (
            FileNameParts[^1] == imageData.FileNameParts()[^1]
            &&
            FileNameParts[^2] == imageData.FileNameParts()[^2]
        )
            return true;

        return false;
    }
}
