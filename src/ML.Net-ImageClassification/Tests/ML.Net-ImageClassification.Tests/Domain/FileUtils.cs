namespace ML.Net.ImageClassification.Tests.Domain;

public class FileUtils
{
    public static IEnumerable<(string imagePath, string label)> LoadImagesFromDirectory(string folder, bool useFolderNameasLabel)
    {
        var imagesPath = Directory
            .GetFiles(folder, "*", searchOption: SearchOption.AllDirectories)
            .Where(x => Path.GetExtension(x) == ".jpg" || Path.GetExtension(x) == ".png");

        return useFolderNameasLabel
            ? imagesPath.Select(imagePath => (imagePath, Directory.GetParent(imagePath).Name))
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

    public static IEnumerable<ImageData> LoadImageDataFromDirectory(string folder, bool useFolderNameAsLabel = true)
    {
        var files = Directory.GetFiles(folder, "*", searchOption: SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (Path.GetExtension(file) != ".jpg" && Path.GetExtension(file) != ".png")
                continue;

            var label = Path.GetFileName(file);
            if (useFolderNameAsLabel)
            {
                label = Directory.GetParent(file)!.Name;
            }

            yield return new ImageData(file, label);
        }
    }



    public static IEnumerable<InMemoryImageData> LoadInMemoryImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        => LoadImagesFromDirectory(folder, useFolderNameAsLabel)
            .Select(x => new InMemoryImageData(
                    image: File.ReadAllBytes(x.imagePath),
                    label: x.label,
                    imageFileName: Path.GetFileName(x.imagePath),
                    fullName: x.imagePath
                ));
}
