using static Tensorflow.Binding;

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
        Dictionary<string, Dictionary<string, string[]>> LoadDataFromDir(string imagesDir, float testingPercentage = 0.2f, float validationPercentage = 0.1f)
        {
            // Look at the folder structure, and create lists of all the images.
            var sub_dirs = tf.gfile.Walk(imagesDir)
                .Select(x => x.Item1)
                .OrderBy(x => x)
                .ToArray();

            var imageDataset = new Dictionary<string, Dictionary<string, string[]>>();

            foreach (var sub_dir in sub_dirs)
            {
                var dir_name = sub_dir.Split(Path.DirectorySeparatorChar).Last();
                logger.Information($"Looking for images in '{dir_name}'");
                var file_list = Directory.GetFiles(sub_dir);
                if (len(file_list) < 20)
                    logger.Information($"WARNING: Folder has less than 20 images, which may cause issues.");

                var label_name = dir_name.ToLower();
                imageDataset[label_name] = new Dictionary<string, string[]>();
                int testing_count = (int)Math.Floor(file_list.Length * testingPercentage);
                int validation_count = (int)Math.Floor(file_list.Length * validationPercentage);
                imageDataset[label_name]["testing"] = file_list.Take(testing_count).ToArray();
                imageDataset[label_name]["validation"] = file_list.Skip(testing_count).Take(validation_count).ToArray();
                imageDataset[label_name]["training"] = file_list.Skip(testing_count + validation_count).ToArray();
            }

            var classCount = len(imageDataset);
            if (classCount == 0)
                logger.Information($"No valid folders of images found at {imagesDir}");
            if (classCount == 1)
                logger.Information("Only one valid folder of images found at " +
                     imagesDir +
                     " - multiple classes are needed for classification.");
            return imageDataset;
        }
    }
}
