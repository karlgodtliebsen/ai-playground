using System.Text;
using ImageClassification.Domain.Models;
using Serilog;

namespace ImageClassification.Domain.Utils;

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

    static readonly char[] CsvSeparator = new[] { ',', '\t' };


    public IEnumerable<TrainingData> LoadTrainingImageToLabelsMap(string inputFolderPath, ImageLabelMapper? mapper, string defaultExtension = ".jpg")
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
        logger.Information("Selecting CSV/TSV mapping file {file}", file);

        var trainingData = File.ReadAllLines(file!);
        foreach (var s in trainingData)
        {
            var data = s.Split(CsvSeparator);
            var fileName = data[mapper.ImageIndex];
            //check extension for filename
            if (Path.GetExtension(fileName) is not ".jpg" and not ".png")
            {
                fileName = $"{fileName}{defaultExtension}";           //extension jpg? Qualified guess
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
        var useFolderNameAsLabel = true;
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
            var sourceParts = imageSetFolderPath.Split(Path.DirectorySeparatorChar);
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

    public IEnumerable<InMemoryImageData> LoadInMemoryImagesFromDirectory(string inputFolderPath, string imageSetFolderPath, ImageLabelMapper? mapper)
    {
        var useFolderNameAsLabel = true;
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
