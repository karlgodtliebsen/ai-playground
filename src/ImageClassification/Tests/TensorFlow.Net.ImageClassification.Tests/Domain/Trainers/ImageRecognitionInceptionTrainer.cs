using System.Diagnostics;

using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Configuration;
using ML.Net.ImageClassification.Tests.Domain.Models;

using Serilog;

using SerilogTimings;
using SerilogTimings.Extensions;

using Tensorflow.NumPy;

using static Tensorflow.Binding;

namespace ML.Net.ImageClassification.Tests.Domain.Trainers;

//Based on:
//https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/ImageRecognitionInception.cs#L69

public sealed class ImageRecognitionInceptionTrainer : ITensorFlowTrainer
{
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;

    private readonly TensorFlowImageClassificationOptions options;
    public ImageRecognitionInceptionTrainer(IOptions<TensorFlowImageClassificationOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);
        var pbModelFile = PathUtils.GetPath(options.InputFilePath, imageSetPath, options.ModelName);
        var labelFile = PathUtils.GetPath(options.InputFilePath, imageSetPath, mapper.LabelFileName);

        tf.compat.v1.disable_eager_execution();

        logger.Information("Starting Training of [{imageSetPath}]...", imageSetPath);
        using Operation op0 = logger.BeginOperation("Load Images for {imageSetPath}...", imageSetPath);
        IList<ImageData> images = imageLoader.LoadImagesMappedToLabelCategory(imageSetFolderPath, inputFolderPath, mapper).ToList();
        op0.Complete();
        logger.Information("Number of Images in Training set {set}: {count}", imageSetPath, images.Count);

        var dataFiles = AddTensorFlow(images).ToList();

        var graph = tf.Graph().as_default();
        //import GraphDef from pb file
        graph.Import(pbModelFile);

        const string input_name = "input";
        const string output_name = "output";

        var input_operation = graph.OperationByName(input_name);
        var output_operation = graph.OperationByName(output_name);

        var sw = new Stopwatch();

        var sess = tf.Session(graph);

        var labels = File.ReadAllLines(labelFile);
        var result_labels = new List<string>();

        foreach (var data in dataFiles)
        {
            logger.Information("Training using {data} - {label}...", data.ImageData.ImagePath, data.ImageData.Label);
            sw.Restart();
            using var op = logger.BeginOperation("Process Image Tensor");
            var results = sess.run(output_operation.outputs[0], (input_operation.outputs[0], data.NdArray));
            results = np.squeeze(results);
            int idx = np.argmax(results);
            string label = labels[idx];
            logger.Information("{label} {name} in {lapsedMilliseconds}ms", data.ImageData.Label, label, sw.ElapsedMilliseconds);
            result_labels.Add(label);
            op.Complete();
        }

        return result_labels.Contains("brocolli").ToString();
    }

    private NDArray ReadTensorFromImageFile(string file_name,
                                            int input_height = 224,
                                            int input_width = 224,
                                            int input_mean = 117,
                                            int input_std = 1)
    {
        var graph = tf.Graph().as_default();

        var file_reader = tf.io.read_file(file_name, "file_reader");
        var decodeJpeg = tf.image.decode_jpeg(file_reader, channels: 3, name: "DecodeJpeg");
        var cast = tf.cast(decodeJpeg, tf.float32);
        var dims_expander = tf.expand_dims(cast, 0);
        var resize = tf.constant(new int[] { input_height, input_width });
        var bilinear = tf.image.resize_bilinear(dims_expander, resize);
        var sub = tf.subtract(bilinear, new float[] { input_mean });
        var normalized = tf.divide(sub, new float[] { input_std });

        var sess = tf.Session(graph);
        return sess.run(normalized);
    }


    private IEnumerable<ImageTensorData> AddTensorFlow(IList<ImageData> images)
    {
        for (var i = 0; i < images.Count; i++)
        {
            var image = images[i];
            var file = image.FullFileName();
            var ndArray = ReadTensorFromImageFile(file);
            yield return new ImageTensorData(ndArray, image);
        }
    }
}

public struct ImageTensorData
{
    public NDArray NdArray { get; }
    public ImageData ImageData { get; }


    public ImageTensorData(NDArray ndArray, ImageData imageData)
    {
        NdArray = ndArray;
        ImageData = imageData;
    }
}
