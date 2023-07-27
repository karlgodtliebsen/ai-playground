using System.Text;

using ML.Net.ImageClassification.Tests.Domain.Models;

using Serilog;

namespace ML.Net.ImageClassification.Tests.Domain;

public sealed class ImageLoader : IImageLoader
{
    private readonly ILogger logger;

    public ImageLoader(ILogger logger)
    {
        this.logger = logger;
    }

    public IEnumerable<(string imagePath, string label)> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel)
    {
        var imagesPath = Directory
            .GetFiles(folder, "*", searchOption: SearchOption.AllDirectories)
            .Where(x => Path.GetExtension(x) == ".jpg" || Path.GetExtension(x) == ".png");

        return useFolderNameAsLabel
            ? imagesPath.Select(imagePath => (imagePath, Directory.GetParent(imagePath)!.Name))
            : imagesPath.Select(imagePath =>
            {
                var label = Path.GetFileName(imagePath);
                for (var index = 0; index < label.Length; index++)
                {
                    if (!char.IsLetter(label[index]))
                    {
                        label = label.Substring(0, index);
                        break;
                    }
                }
                return (imagePath, label);
            });
    }

    public IEnumerable<ImageData> LoadImageDataFromDirectory(string folder, bool useFolderNameAsLabel = true)
    {
        var files = Directory.GetFiles(folder, "*", searchOption: SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);
            if (extension != ".jpg" && extension != ".png")
                continue;

            var imageName = Path.GetFileName(file);
            var label = imageName;
            if (useFolderNameAsLabel)
            {
                label = Directory.GetParent(file)!.Name;
                imageName = Path.Combine(label, imageName);
            }
            yield return new ImageData(file, label, imageName);
        }
    }


    public IEnumerable<TrainingData> LoadTrainingImageToLabelsMap(string inputFolderPath, ImageLabelMapper? mapper)
    {
        if (mapper is null)
        {
            yield break;
        }
        logger.Information("Looking for Image to Labels mapping data from folder: {inputFolderPath} {file}", inputFolderPath, mapper.FileName);
        var file = Path.Combine(inputFolderPath, mapper.FileName);
        if (!File.Exists(file))
        {
            logger.Error("Mapping file {file} does not exist", file);
            yield break;
        }
        logger.Information("Selecting CSV mapping file {file}", file);
        var trainingData = File.ReadAllLines(file!);
        foreach (var s in trainingData)
        {
            var data = s.Split(',');
            var fileName = data[mapper.ImageIndex];
            //check extension for filename
            if (Path.GetExtension(fileName) is not ".jpg" and not ".png")
            {
                fileName = $"{fileName}.jpg";           //Qualified guess
            }
            string? label = default;
            if (mapper.LabelIndex.HasValue)
            {
                label = data[mapper.LabelIndex.Value];
            }
            else
            {
                var sb = new StringBuilder();
                for (var index = mapper.LabelIndexRange!.Value.from; index <= mapper.LabelIndexRange!.Value.to; index++)
                {
                    if (index < data.Length)
                    {
                        sb.AppendLine(data[index]);
                    }
                }
                label = sb.ToString();
            }
            yield return new TrainingData(fileName, label);
        }
    }


    public IEnumerable<ImageData> LoadImagesMappedToLabelCategory(string imageSetFolderPath, string inputFolderPath, ImageLabelMapper? mapper)
    {
        bool useFolderNameAsLabel = true;
        IList<TrainingData>? data = null;
        if (mapper is not null)
        {
            data = LoadTrainingImageToLabelsMap(inputFolderPath, mapper).ToList();
            if (data.Count > 0)
            {
                useFolderNameAsLabel = false;
            }
        }
        logger.Information("Loading images from folder: {imageSetFolderPath}", imageSetFolderPath);
        var images = LoadImageDataFromDirectory(folder: imageSetFolderPath, useFolderNameAsLabel: useFolderNameAsLabel).ToList();
        if (useFolderNameAsLabel)
        {
            foreach (var imageData in images)
            {
                yield return imageData;
            }
        }
        else
        {
            string[] sourceParts = imageSetFolderPath.Split(Path.DirectorySeparatorChar);
            foreach (var imageData in images)
            {
                if (imageData.FindMatchingLabelForImage(data, sourceParts, imageSetFolderPath))
                {
                    yield return imageData;
                }
                else
                {
                    logger.Warning("No matching label found for image {@imageData}", imageData);
                }
            }
        }
    }

    //var trainingData = data!.FirstOrDefault(x => x.FindMatchingLabelForImage(imageData));
    //if (trainingData is not null)
    //{
    //    imageData.MergeDetails(trainingData, sourceParts, imageSetFolderPath);
    //    if (VerifyFile(imageSetFolderPath, imageData))
    //    {
    //        yield return imageData;
    //    }
    //}
    //else
    //{
    //    var imageFile = imageData.FullFileName();
    //    if (File.Exists(imageFile))
    //    {
    //        var parentDirectory = Directory.GetParent(imageData.FullFileName());
    //        var diff = imageFile.Substring(imageSetFolderPath.Length);
    //        imageData.ImagePath = diff.TrimStart(Path.DirectorySeparatorChar);
    //        if (VerifyFile(imageSetFolderPath, imageData))
    //        {
    //            yield return imageData;
    //        }
    //    }
    //    else
    //    {

    //        logger.Warning("No matching label found for image {@imageData}", imageData);
    //    }
    //}
    //private bool VerifyFile(string imageSetFolderPath, ImageData imageData)
    //{
    //    var imageFile = Path.Combine(imageSetFolderPath, imageData.ImagePath);
    //    if (File.Exists(imageFile))
    //    {
    //        return true;
    //    }
    //    logger.Warning("Incorrect Directory/File Level for Label Generation found for image {@imageData}", imageData);
    //    return false;
    //}

    public IEnumerable<InMemoryImageData> LoadInMemoryImagesFromDirectory(string inputFolderPath, string imageSetFolderPath, ImageLabelMapper? mapper)
    {
        bool useFolderNameAsLabel = true;
        IList<TrainingData>? data = null;
        if (mapper is not null)
        {
            data = LoadTrainingImageToLabelsMap(inputFolderPath, mapper).ToList();
            if (data.Count > 0)
            {
                useFolderNameAsLabel = false;
            }
        }
        var images = LoadImagesFromDirectory(imageSetFolderPath, useFolderNameAsLabel)
                                                .Select(x => new InMemoryImageData(
                                                        image: File.ReadAllBytes(x.imagePath),
                                                        label: x.label,
                                                        imageFileName: Path.GetFileName(x.imagePath),
                                                        fullName: x.imagePath
                                                    ));
        if (useFolderNameAsLabel)
        {
            foreach (var imageData in images)
            {
                yield return imageData;
            }
        }
        else
        {
            foreach (var imageData in images)
            {
                var d = data!.FirstOrDefault(x => x.FindMatchingLabelForImage(imageData));
                if (d is not null)
                {
                    imageData.Label = d.Label;
                }
                yield return imageData;
            }
        }

    }
}
