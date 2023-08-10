namespace ImageClassification.Domain.Models;

public sealed class ImageData
{
    private readonly string fullFileName;
    private readonly string[] fileNameParts;

    public ImageData(string fullFileName, string label, string imagePath)
    {
        this.fullFileName = fullFileName;
        Label = label;
        fileNameParts = fullFileName.Split(Path.DirectorySeparatorChar);
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
    /// <summary>
    /// Transfer Label from TrainingData to ImageData
    /// Compensate for the differences in image folders and info in dataSet, where the image path combined with the label and subfolder path does not align
    /// </summary>
    /// <param name="trainingData"></param>
    /// <param name="sourceParts"></param>
    /// <param name="folder"></param>
    public void MergeDetails(TrainingData trainingData, string[] sourceParts, string folder)
    {
        Label = trainingData.Label;
        if (FileNameParts().Length > sourceParts.Length + 1)
        {
            var f = FullFileName().Substring(folder.Length);
            ImagePath = f.Substring(1);
        }
    }

    public string Label { get; set; }
    public string LabelAsDir
    {
        get
        {
            return Label.Replace("\n", "").Replace(" ", "-").Replace("\r", "");
        }
    }

    public string ImagePath { get; set; }

    public bool FindMatchingLabelForImage(IEnumerable<TrainingData> data, string[] sourceParts, string folder)
    {
        var trainingData = data!.FirstOrDefault(x => x.FindMatchingLabelForImage(this));
        if (trainingData is not null)
        {
            MergeDetails(trainingData, sourceParts, folder);
            if (VerifyFile(folder))
            {
                return true;
            }
        }
        else
        {
            var imageFile = FullFileName();
            if (File.Exists(imageFile))
            {
                var diff = imageFile.Substring(folder.Length);
                ImagePath = diff.TrimStart(Path.DirectorySeparatorChar);
                if (VerifyFile(folder))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool VerifyFile(string imageSetFolderPath)
    {
        var imageFile = Path.Combine(imageSetFolderPath, ImagePath);
        if (File.Exists(imageFile))
        {
            return true;
        }
        return false;
    }
}

