using ImageClassification.Domain.Models;

namespace ImageClassification.Domain.Utils;

public interface IImageLoader
{
    IEnumerable<(string imagePath, string label)> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel);
    IEnumerable<ImageData> LoadImageDataFromDirectory(string folder, bool useFolderNameAsLabel = true);
    IEnumerable<TrainingData> LoadTrainingImageToLabelsMap(string inputFolderPath, ImageLabelMapper? mapper = null, string defaultExtension = ".jpg");
    IEnumerable<ImageData> LoadImagesMappedToLabelCategory(string imageSetFolderPath, string inputFolderPath, ImageLabelMapper? mapper = null);
    IEnumerable<InMemoryImageData> LoadInMemoryImagesFromDirectory(string inputFolderPath, string imageSetFolderPath, ImageLabelMapper? mapper = null);
}
