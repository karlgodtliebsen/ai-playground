using ML.Net.ImageClassification.Tests.Domain.Models;

namespace ML.Net.ImageClassification.Tests.Domain;

public interface IImageLoader
{
    IEnumerable<(string imagePath, string label)> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel);
    IEnumerable<ImageData> LoadImageDataFromDirectory(string folder, bool useFolderNameAsLabel = true);
    IEnumerable<TrainingData> LoadTrainingImageToLabelsMap(string inputFolderPath, ImageLabelMapper? mapper);
    IEnumerable<ImageData> LoadImagesMappedToLabelCategory(string imageSetFolderPath, string inputFolderPath, ImageLabelMapper? mapper);
    IEnumerable<InMemoryImageData> LoadInMemoryImagesFromDirectory(string inputFolderPath, string imageSetFolderPath, ImageLabelMapper? mapper);
}
