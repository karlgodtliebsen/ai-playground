using SciSharp.Models;
using SciSharp.Models.Exceptions;

using Tensorflow;
using Tensorflow.NumPy;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.TransferLearning
{
    public partial class ExtendedTransferLearning
    {
        Session predictSession = null!;
        public ModelPredictResult Predict(Tensor input)
        {
            if (!File.Exists(options.ModelPath))
                throw new FreezedGraphNotFoundException();

            if (labels == null)
                labels = File.ReadAllLines(options.LabelPath);

            // import graph and variables
            if (predictSession == null)
            {
                var graph = tf.Graph().as_default();
                graph.Import(options.ModelPath);
                predictSession = tf.Session(graph);
                tf.Context.restore_mode();
            }

            var (input_tensor, output_tensor) = GetInputOutputTensors();
            var result = predictSession.run(output_tensor, (input_tensor, input));

            var prob = np.squeeze(result);
            var idx = np.argmax(prob);

            logger.Information($"Predicted result: {labels[idx]} - {(float)prob[idx] / 100:P}");


            return new ModelPredictResult
            {
                Label = labels[idx],
                Probability = prob[idx]
            };
        }

        (Tensor, Tensor) GetInputOutputTensors()
        {
            Tensor input_tensor = predictSession.graph.OperationByName(input_tensor_name);
            Tensor output_tensor = predictSession.graph.OperationByName(final_tensor_name);
            return (input_tensor, output_tensor);
        }
    }
}
