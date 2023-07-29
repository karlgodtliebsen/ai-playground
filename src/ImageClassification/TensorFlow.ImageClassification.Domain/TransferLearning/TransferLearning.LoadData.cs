using ImageClassification.Domain.Models;

namespace ImageClassification.Domain.TransferLearning
{
    public partial class ExtendedTransferLearning
    {
        /// <summary>
        /// Builds a list of training images from the file system.
        /// </summary>
        /// <param name="imagesDir"></param>
        /// <param name="testingPercentage"></param>
        /// <param name="validationPercentage"></param>
        /// <returns></returns>
        void LoadDataFromDir(string imagesDir, float testingPercentage = 0.2f, float validationPercentage = 0.1f)
        {
            var images = imageLoader.LoadImagesMappedToLabelCategory(options.DataDir, options.InputFolderPath!, options.Mapper).ToList();
            var subDirs = images.Select(x => x.Label).Distinct().ToList();
            var imageDataSet = new Dictionary<string, IDictionary<string, ImageData[]>>();

            foreach (var subDir in subDirs)
            {
                var dirName = subDir.Split(Path.DirectorySeparatorChar).Last();
                logger.Information("Looking for images in '{dirName}'", dirName);
                var fileList = images.Where(x => x.Label == subDir).ToList();
                var fileCount = fileList.Count;
                if (fileCount < 20)
                {
                    logger.Warning("WARNING: Folder {dirName} has less than 20 images, which may cause issues.", dirName);
                }

                var labelName = dirName.ToLower();
                int testingCount = (int)Math.Floor(fileCount * testingPercentage);
                int validationCount = (int)Math.Floor(fileCount * validationPercentage);

                imageDataSet[labelName] = new Dictionary<string, ImageData[]>();
                imageDataSet[labelName]["testing"] = fileList.Take(testingCount).ToArray();
                imageDataSet[labelName]["validation"] = fileList.Skip(testingCount).Take(validationCount).ToArray();
                imageDataSet[labelName]["training"] = fileList.Skip(testingCount + validationCount).ToArray();
            }

            var classCount = imageDataSet.Count;
            if (classCount == 0)
            {
                logger.Information("No valid folders of images found at {imagesDir}", imagesDir);
            }

            if (classCount == 1)
            {
                logger.Information("Only one valid folder of images found at {imagesDir} - multiple classes are needed for classification.", imagesDir);
            }

            image_dataset = imageDataSet;
        }
    }
}
