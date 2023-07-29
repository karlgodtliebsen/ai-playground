﻿using System.Diagnostics;

using ImageClassification.Domain.Models;

using SciSharp.Models;

using Tensorflow;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.TransferLearning
{
    public partial class ExtendedTransferLearning
    {
        IDictionary<string, IDictionary<string, ImageData[]>> image_dataset = null!;
        Tensor resized_image_tensor = null!;
        Tensor bottleneck_tensor = null!;
        string tfhub_module = "https://tfhub.dev/google/imagenet/inception_v3/feature_vector/3";
        Tensor final_tensor = null!;
        Tensor ground_truth_input = null!;

        Operation train_step = null!;
        Tensor bottleneck_input = null!;
        Tensor cross_entropy = null!;

        // The location where variable checkpoints will be stored.

        string input_tensor_name = "Placeholder";
        string final_tensor_name = "Score";
        float learning_rate = 0.01f;

        int eval_step_interval = 10;

        int test_batch_size = -1;
        int validation_batch_size = 100;
        int intermediate_store_frequency = 0;
        const int MAX_NUM_IMAGES_PER_CLASS = 134217727;

        public void Train(TrainingOptions options)
        {
            LoadDataFromDir(this.options.DataDir, testingPercentage: this.options.TestingPercentage, validationPercentage: this.options.ValidationPercentage);

            var sw = new Stopwatch();
            var graph = isImportingGraph ? ImportGraph() : BuildGraph();
            var sess = tf.Session(graph);

            // Initialize all weights: for the module to their pretrained values,
            // and for the newly added retraining layer to random initial values.
            var init = tf.global_variables_initializer();
            sess.run(init);

            var (jpeg_data_tensor, decoded_image_tensor) = add_jpeg_decoding();

            // We'll make sure we've calculated the 'bottleneck' image summaries and
            // cached them on disk.
            cache_bottlenecks(sess, image_dataset, bottleneckDir, jpeg_data_tensor, decoded_image_tensor, resized_image_tensor, bottleneck_tensor, tfhub_module);

            // Create the operations we need to evaluate the accuracy of our new layer.
            var (evaluation_step, _) = add_evaluation_step(final_tensor, ground_truth_input);

            // Merge all the summaries and write them out to the summaries_dir
            var merged = tf.summary.merge_all();
            var train_writer = tf.summary.FileWriter(summariesDir + "/train", sess.graph);
            var validation_writer = tf.summary.FileWriter(summariesDir + "/validation", sess.graph);

            // Create a train saver that is used to restore values into an eval graph
            // when exporting models.
            var train_saver = tf.train.Saver();
            var checkpoint = Path.Combine(taskDir, "checkpoint");
            train_saver.save(sess, checkpoint);

            sw.Restart();

            for (int i = 0; i < options.TrainingSteps; i++)
            {
                var (trainBottlenecks, trainGroundTruth, _) = get_random_cached_bottlenecks(
                        sess, image_dataset, options.BatchSize, "training",
                        bottleneckDir, jpeg_data_tensor,
                        decoded_image_tensor, resized_image_tensor, bottleneck_tensor,
                        tfhub_module);

                // Feed the bottlenecks and ground truth into the graph, and run a training
                // step. Capture training summaries for TensorBoard with the `merged` op.
                var results = sess.run(
                        new ITensorOrOperation[] { merged, train_step },
                        new FeedItem(bottleneck_input, trainBottlenecks),
                        new FeedItem(ground_truth_input, trainGroundTruth));
                var train_summary = results[0];

                // TODO
                // train_writer.add_summary(train_summary, i);

                // Every so often, print out how well the graph is training.
                bool is_last_step = (i + 1 == options.TrainingSteps);
                if ((i % eval_step_interval) == 0 || is_last_step)
                {
                    (float train_accuracy, float cross_entropy_value) = sess.run((evaluation_step, cross_entropy),
                        (bottleneck_input, trainBottlenecks),
                        (ground_truth_input, trainGroundTruth));

                    var (validation_bottlenecks, validation_ground_truth, _) = get_random_cached_bottlenecks(
                        sess, image_dataset, validation_batch_size, "validation",
                        bottleneckDir, jpeg_data_tensor,
                        decoded_image_tensor, resized_image_tensor, bottleneck_tensor,
                        tfhub_module);

                    // Run a validation step and capture training summaries for TensorBoard
                    // with the `merged` op.
                    (_, float validation_accuracy) = sess.run((merged, evaluation_step),
                        (bottleneck_input, validation_bottlenecks),
                        (ground_truth_input, validation_ground_truth));

                    // validation_writer.add_summary(validation_summary, i);
                    logger.Information("Step {i}: Training accuracy = {train_accuracy}%, Cross entropy = {cross_entropy_value}, Validation accuracy = {validation_accuracy}% (N={validation_bottlenecks}) {elapsedMilliseconds}ms"
                                    , i, train_accuracy * 100, cross_entropy_value.ToString("G4"), validation_accuracy * 100, len(validation_bottlenecks), sw.ElapsedMilliseconds);
                    sw.Restart();
                }

                // Store intermediate results
                int intermediateFrequency = intermediate_store_frequency;
                if (intermediateFrequency > 0 && i % intermediateFrequency == 0 && i > 0)
                {

                }
            }

            // After training is complete, force one last save of the train checkpoint.
            logger.Information("Saving checkpoint to {checkpoint}", checkpoint);
            train_saver.save(sess, checkpoint);

            SaveModel();
        }

        private (Tensor, Tensor) add_jpeg_decoding()
        {
            // height, width, depth
            var input_dim = (299, 299, 3);
            var jpeg_data = tf.placeholder(tf.@string, name: "DecodeJPGInput");
            var decoded_image = tf.image.decode_jpeg(jpeg_data, channels: input_dim.Item3);
            // Convert from full range of uint8 to range [0,1] of float32.
            var decoded_image_as_float = tf.image.convert_image_dtype(decoded_image, tf.float32);
            var decoded_image_4d = tf.expand_dims(decoded_image_as_float, 0);
            var resize_shape = tf.stack(new int[] { input_dim.Item1, input_dim.Item2 });
            var resize_shape_as_int = tf.cast(resize_shape, dtype: tf.int32);
            var resized_image = tf.image.resize_bilinear(decoded_image_4d, resize_shape_as_int);
            return (jpeg_data, resized_image);
        }

        (Tensor, Tensor, bool) create_module_graph(Graph graph)
        {
            tf.train.import_meta_graph($"{options.MetaDataPath}/{options.MetaDataFilename}");
            Tensor resized_image_tensor = graph.OperationByName(input_tensor_name);
            Tensor bottleneck_tensor = graph.OperationByName("module_apply_default/hub_output/feature_vector/SpatialSqueeze");
            var wants_quantization = false;
            return (bottleneck_tensor, resized_image_tensor, wants_quantization);
        }
    }
}
