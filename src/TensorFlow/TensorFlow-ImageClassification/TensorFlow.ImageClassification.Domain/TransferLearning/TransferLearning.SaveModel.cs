using Google.Protobuf;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.TransferLearning
{
    public partial class TransferLearningExtended
    {
        void SaveModel()
        {
            // Write out the trained graph and labels with the weights stored as
            // constants.
            logger.Information($"Saving final result to: {options.ModelPath}");
            var (sess, _, _, _, _, _) = build_eval_session();
            var graph = sess.graph;
            var output_graph_def = tf.graph_util.convert_variables_to_constants(
                sess, graph.as_graph_def(), new string[] { final_tensor_name });
            File.WriteAllBytes(options.ModelPath, output_graph_def.ToByteArray());
            File.WriteAllText(options.LabelPath, string.Join("\n", image_dataset.Keys));
        }
    }
}
