using ImageClassification.Domain.Models;

using Tensorflow;
using Tensorflow.NumPy;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.TransferLearning
{
    public partial class TransferLearningExtended
    {
        /// <summary>
        /// Ensures all the training, testing, and validation bottlenecks are cached.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="image_lists"></param>
        /// <param name="bottleneck_dir"></param>
        /// <param name="jpeg_data_tensor"></param>
        /// <param name="decoded_image_tensor"></param>
        /// <param name="resized_input_tensor"></param>
        /// <param name="bottleneck_tensor"></param>
        /// <param name="module_name"></param>
        void cache_bottlenecks(Session session, IDictionary<string, IDictionary<string, ImageData[]>> image_lists,
                                string bottleneck_dir, Tensor jpeg_data_tensor, Tensor decoded_image_tensor,
                                Tensor resized_input_tensor, Tensor bottleneck_tensor, string module_name)
        {
            int how_many_bottlenecks = 0;
            var kvs = image_lists.ToArray();
            var categories = new string[] { "training", "testing", "validation" };
            for (var i = 0; i < kvs.Length; i++)
            {
                var (label_name, label_lists) = (kvs[i].Key, kvs[i].Value);
                var sub_dir_path = Path.Combine(bottleneck_dir, label_name.Replace("\n", "").Replace(" ", "-").Replace("\r", ""));
                Directory.CreateDirectory(sub_dir_path);

                for (var j = 0; j < categories.Length; j++)
                {
                    var category = categories[j];
                    var category_list = label_lists[category];
                    foreach (var (index, _) in enumerate(category_list))
                    {
                        get_or_create_bottleneck(session, image_lists, label_name, index, category,
                            bottleneck_dir, jpeg_data_tensor, decoded_image_tensor,
                            resized_input_tensor, bottleneck_tensor, module_name);
                        how_many_bottlenecks++;
                        if (how_many_bottlenecks % 300 == 0)
                        {
                            logger.Information("{numberOfBottlenecks} bottleneck files created.", how_many_bottlenecks);
                        }
                    }
                }
            }
        }

        float[] get_or_create_bottleneck(Session session, IDictionary<string, IDictionary<string, ImageData[]>> image_lists,
                                            string label_name, int index, string category, string bottleneck_dir,
                                            Tensor jpeg_data_tensor, Tensor decoded_image_tensor, Tensor resized_input_tensor,
                                            Tensor bottleneck_tensor, string module_name)
        {
            var label_lists = image_lists[label_name];
            string bottleneck_path = get_bottleneck_path(image_lists, label_name, bottleneck_dir, index, category, module_name);
            if (!File.Exists(bottleneck_path))
                return create_bottleneck_file(bottleneck_path, image_lists, label_name, index,
                                       category, session, jpeg_data_tensor,
                                       decoded_image_tensor, resized_input_tensor,
                                       bottleneck_tensor);
            var bottleneck_string = File.ReadAllText(bottleneck_path);
            var bottleneck_values = Array.ConvertAll(bottleneck_string.Split(' '), x => float.Parse(x));
            return bottleneck_values;
        }

        float[] create_bottleneck_file(string bottleneck_path, IDictionary<string, IDictionary<string, ImageData[]>> image_lists,
                                        string label_name, int index, string category, Session session,
                                        Tensor jpeg_data_tensor, Tensor decoded_image_tensor, Tensor resized_input_tensor, Tensor bottleneck_tensor)
        {
            // Create a single bottleneck file.
            logger.Information("Creating bottleneck at {bottleneck_path}", bottleneck_path);
            var image_path = get_image_path(image_lists, label_name, options.DataDir, index, category);
            if (!File.Exists(image_path))
            {
                logger.Information("File does not exist {image_path}", image_path);
            }
            var image_data = File.ReadAllBytes(image_path);
            var bottleneck_values = run_bottleneck_on_image(session, image_data, jpeg_data_tensor, decoded_image_tensor, resized_input_tensor, bottleneck_tensor);
            var values = bottleneck_values.ToArray<float>();
            var bottleneck_string = string.Join(" ", values);
            File.WriteAllText(bottleneck_path, bottleneck_string);
            return values;
        }

        /// <summary>
        /// Runs inference on an image to extract the 'bottleneck' summary layer.
        /// </summary>
        /// <param name="session">Current active TensorFlow Session.</param>
        /// <param name="image_data">Data of raw JPEG data.</param>
        /// <param name="image_data_tensor">Input data layer in the graph.</param>
        /// <param name="decoded_image_tensor">Output of initial image resizing and preprocessing.</param>
        /// <param name="resized_input_tensor">The input node of the recognition graph.</param>
        /// <param name="bottleneck_tensor">Layer before the final softmax.</param>
        /// <returns></returns>
        NDArray run_bottleneck_on_image(Session session, byte[] image_data, Tensor image_data_tensor, Tensor decoded_image_tensor, Tensor resized_input_tensor, Tensor bottleneck_tensor)
        {
            // First decode the JPEG image, resize it, and rescale the pixel values.
            var resized_input_values = session.run(decoded_image_tensor, new FeedItem(image_data_tensor, new NDArray(image_data, image_data_tensor.shape, dtype: tf.@string)));
            // Then run it through the recognition network.
            var bottleneck_values = session.run(bottleneck_tensor, new FeedItem(resized_input_tensor, resized_input_values))[0];
            bottleneck_values = np.squeeze(bottleneck_values);
            return bottleneck_values;
        }

        string get_image_path(IDictionary<string, IDictionary<string, ImageData[]>> image_lists, string label_name, string image_dir, int index, string category)
        {
            if (!image_lists.ContainsKey(label_name))
            {
                logger.Information("Label does not exist {label_name}", label_name);
            }

            var label_lists = image_lists[label_name];
            if (!label_lists.ContainsKey(category))
            {
                logger.Information("Category does not exist {category}", category);
            }
            var category_list = label_lists[category];
            if (category_list.Length == 0)
            {
                logger.Information("Label {label_name} has no images in the category {category}.", label_name, category);
            }
            var mod_index = index % len(category_list);
            var full_path = Path.Combine(image_dir, category_list[mod_index].ImagePath);
            return full_path;
        }

        string get_bottleneck_path(IDictionary<string, IDictionary<string, ImageData[]>> image_lists, string label_name, string bottleneck_dir, int index, string category, string module_name)
        {
            module_name = module_name
                        .Replace("://", "~")  // URL scheme.
                        .Replace('/', '~')    // URL and Unix paths.
                        .Replace(':', '~')
                        .Replace('\\', '~')   // Windows paths.
                        ;
            var imagePath = get_image_path(image_lists, label_name, bottleneck_dir, index, category);
            imagePath = imagePath.Replace(' ', '_');
            var fullName = imagePath + "_" + module_name + ".txt";
            return fullName;
        }

        (NDArray, long[], string[]) get_random_cached_bottlenecks(Session session, IDictionary<string, IDictionary<string, ImageData[]>> image_lists,
            int how_many, string category, string bottleneck_dir,
            Tensor jpeg_data_tensor, Tensor decoded_image_tensor, Tensor resized_input_tensor,
            Tensor bottleneck_tensor, string module_name)
        {
            float[,] bottlenecks;
            var ground_truths = new List<long>();
            var filenames = new List<string>();
            var class_count = image_lists.Keys.Count;
            if (how_many >= 0)
            {
                bottlenecks = new float[how_many, 2048];
                // Retrieve a random sample of bottlenecks.
                foreach (var unused_i in range(how_many))
                {
                    int label_index = new Random().Next(class_count);
                    string label_name = image_lists.Keys.ToArray()[label_index];
                    int image_index = new Random().Next(MAX_NUM_IMAGES_PER_CLASS);
                    string image_name = get_image_path(image_lists, label_name, bottleneck_dir, image_index, category);
                    var bottleneck = get_or_create_bottleneck(
                                                              session, image_lists, label_name, image_index, category,
                                                              bottleneck_dir, jpeg_data_tensor, decoded_image_tensor,
                                                              resized_input_tensor, bottleneck_tensor, module_name);
                    for (int col = 0; col < bottleneck.Length; col++)
                    {
                        bottlenecks[unused_i, col] = bottleneck[col];
                    }
                    ground_truths.Add(label_index);
                    filenames.Add(image_name);
                }
            }
            else
            {
                how_many = 0;
                // Retrieve all bottlenecks.
                foreach (var (label_index, label_name) in enumerate(image_lists.Keys.ToArray()))
                {
                    how_many += image_lists[label_name][category].Length;
                }
                bottlenecks = new float[how_many, 2048];
                var row = 0;
                foreach (var (label_index, label_name) in enumerate(image_lists.Keys.ToArray()))
                {
                    foreach (var (image_index, imageData) in enumerate(image_lists[label_name][category]))
                    {
                        var bottleneck = get_or_create_bottleneck(
                            session, image_lists, label_name, image_index, category,
                            bottleneck_dir, jpeg_data_tensor, decoded_image_tensor,
                            resized_input_tensor, bottleneck_tensor, module_name);

                        for (int col = 0; col < bottleneck.Length; col++)
                        {
                            bottlenecks[row, col] = bottleneck[col];
                        }
                        row++;
                        ground_truths.Add(label_index);
                        filenames.Add(imageData.FullFileName());
                    }
                }
            }

            return (bottlenecks, ground_truths.ToArray(), filenames.ToArray());
        }
    }
}
