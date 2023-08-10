using SciSharp.Models;
using SciSharp.Models.Exceptions;

using Tensorflow;
using Tensorflow.NumPy;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.TransferLearning
{
    public partial class ExtendedTransferLearning
    {
        private Session? predictSession = null!;

        public ModelPredictResult Predict(Tensor input)
        {
            if (!File.Exists(options.ModelPath))
                throw new FreezedGraphNotFoundException();

            labels ??= File.ReadAllLines(options.LabelPath);

            // import graph and variables
            if (predictSession is null)
            {
                var graph = tf.Graph().as_default();
                graph.Import(options.ModelPath);
                predictSession = tf.Session(graph);
                tf.Context.restore_mode();
            }

            var (inputTensor, outputTensor) = GetInputOutputTensors();
            var result = predictSession.run(outputTensor, (inputTensor, input));

            var prob = np.squeeze(result);
            var idx = np.argmax(prob);
            logger.Information("Predicted result: {label} - {p:P}", labels[idx], (float)prob[idx] / 100);
            return new ModelPredictResult
            {
                Label = labels[idx],
                Probability = prob[idx]
            };
        }

        private (Tensor, Tensor) GetInputOutputTensors()
        {
            Tensor input_tensor = predictSession!.graph.OperationByName(input_tensor_name);
            Tensor output_tensor = predictSession!.graph.OperationByName(final_tensor_name);
            return (input_tensor, output_tensor);
        }
    }
}
