using System.Diagnostics;

using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Configuration;

using Serilog;

using Tensorflow.NumPy;

using static Tensorflow.Binding;

namespace ML.Net.ImageClassification.Tests.Domain;

//Based on:
//https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/ImageRecognitionInception.cs#L69

public sealed class Trainer : ITrainer
{
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;

    private readonly TensorFlowImageClassificationOptions options;
    public Trainer(IOptions<TensorFlowImageClassificationOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.logger = logger;
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

    //string dir = "ImageRecognitionInception";

    string labelFile = "imagenet_comp_graph_label_strings.txt";

    List<NDArray> file_ndarrays = new List<NDArray>();

    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {

        var pbModelFile = PathUtils.GetPath(options.OutputFilePath, imageSetPath, options.ModelName);
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        var labelFile = PathUtils.GetPath(options.InputFilePath, mapper.FileName);


        tf.compat.v1.disable_eager_execution();

        LoadImageFiles(imageSetPath);

        var graph = tf.Graph().as_default();
        //import GraphDef from pb file
        graph.Import(pbModelFile);

        var input_name = "input";
        var output_name = "output";

        var input_operation = graph.OperationByName(input_name);
        var output_operation = graph.OperationByName(output_name);

        var labels = File.ReadAllLines(labelFile);
        var result_labels = new List<string>();
        var sw = new Stopwatch();

        var sess = tf.Session(graph);
        foreach (var nd in file_ndarrays)
        {
            //logger.Information("Starting Training of [{imageSetPath}]...", imageSetPath);
            sw.Restart();
            //using Operation op0 = logger.BeginOperation("Load Images for {imageSetPath}...", imageSetPath);
            var results = sess.run(output_operation.outputs[0], (input_operation.outputs[0], nd));
            results = np.squeeze(results);
            int idx = np.argmax(results);

            logger.Information("{labels} {results} in {lapsedMilliseconds}ms", labels[idx], results[idx], sw.ElapsedMilliseconds);
            result_labels.Add(labels[idx]);
            //op0.Complete();
        }


        return result_labels.Contains("military uniform").ToString();
    }

    private void LoadImageFiles(string imageSetPath)
    {
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        var inputFolderPath = PathUtils.GetPath(options.InputFilePath, imageSetPath);

        //IImageLoader
        //LoadImagesFromDirectory
        // load image file
        //var files = Directory.GetFiles(imageSetPath);
        var files = imageLoader.LoadImagesFromDirectory(imageSetFolderPath, false).Select(x => x.imagePath).ToArray();

        for (int i = 0; i < files.Length; i++)
        {
            var nd = ReadTensorFromImageFile(files[i]);
            file_ndarrays.Add(nd);
        }
    }
}
