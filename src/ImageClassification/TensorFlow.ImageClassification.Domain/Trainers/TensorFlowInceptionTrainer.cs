using System.Diagnostics;

using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using Serilog;

using SerilogTimings.Extensions;

using Tensorflow.NumPy;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.Trainers;

//Based on:
//https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/ImageRecognitionInception.cs#L69

public sealed class TensorFlowInceptionTrainer : ITensorFlowTrainer
{
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;

    private readonly TensorFlowOptions options;
    public TensorFlowInceptionTrainer(IOptions<TensorFlowOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);
        var pbModelFile = PathUtils.GetPath(options.ModelFilePath, imageSetPath, options.ModelName);
        if (!File.Exists(pbModelFile))
        {
            throw new FileNotFoundException("Model file not found", pbModelFile);
        }

        tf.compat.v1.disable_eager_execution();

        logger.Information("Starting Training of [{imageSetPath}]...", imageSetPath);
        using var op0 = logger.BeginOperation("Load Images for {imageSetPath}...", imageSetPath);
        var images = imageLoader.LoadImagesMappedToLabelCategory(imageSetFolderPath, inputFolderPath, mapper).ToList();
        op0.Complete();
        logger.Information("Number of Images in Training set {set}: {count}", imageSetPath, images.Count);

        var dataFiles = AddTensorFlow(images).ToList();

        var graph = tf.Graph().as_default();
        //import GraphDef from pb file
        graph.Import(pbModelFile);

        const string inputName = "input";
        const string outputName = "output";

        var inputOperation = graph.OperationByName(inputName);
        var outputOperation = graph.OperationByName(outputName);

        var sw = new Stopwatch();

        var session = tf.Session(graph);
        var labels = FetchLabels(imageSetPath, mapper);
        var resultLabels = new List<string>();
        foreach (var data in dataFiles)
        {
            logger.Information("Training using {data} - {label}...", data.ImageData.ImagePath, data.ImageData.LabelAsDir);
            sw.Restart();
            using var op = logger.BeginOperation("Process Image Tensor");
            var results = session.run(outputOperation.outputs[0], (inputOperation.outputs[0], data.NdArray));
            results = np.squeeze(results);
            int idx = np.argmax(results);
            var label = GetLabel(labels, idx, data);
            logger.Information("{label} {name} in {lapsedMilliseconds}ms", data.ImageData.LabelAsDir, label, sw.ElapsedMilliseconds);
            resultLabels.Add(label);
            op.Complete();
        }

        return resultLabels.Contains("brocolli").ToString();
    }

    private string GetLabel(string[]? labels, int idx, ImageTensorData data)
    {
        if (labels is not null)
        {
            return labels[idx];
        }
        else
        {
            return data.ImageData.Label;
        }
    }

    private string[]? FetchLabels(string imageSetPath, ImageLabelMapper? mapper)
    {
        if (mapper is not null || mapper?.LabelFileName is not null)
        {
            string labelFile = PathUtils.GetPath(options.InputFilePath, imageSetPath, mapper.LabelFileName);
            if (File.Exists(labelFile))
            {
                return File.ReadAllLines(labelFile);
            }
        }
        return null;
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
