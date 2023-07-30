using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Models;
using ImageClassification.Domain.TransferLearning;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using SciSharp.Models;

using Serilog;

using Tensorflow;

using static Tensorflow.Binding;

namespace ImageClassification.Domain.Predictors;

//https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/image-classification
//TODO: use the predict pool
//For improved performance and thread safety in production environments,
//use the PredictionEnginePool service, which creates an ObjectPool of
//PredictionEngine objects for use throughout your application.
//See this guide on how to use PredictionEnginePool in an ASP.NET Core Web API.
public sealed class TensorFlowTransferLearningInceptionPredictor : IPredictor
{
    private readonly ExtendedModelFactory factory;
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;
    private readonly TensorFlowOptions tensorFlowOptions;
    private readonly ExtendedTaskOptions taskOptions;


    public TensorFlowTransferLearningInceptionPredictor(ExtendedModelFactory factory,
          IOptions<TensorFlowOptions> tensorFlowOptions,
          IOptions<ExtendedTaskOptions> taskOptions,
          IImageLoader imageLoader, ILogger logger)
    {
        this.tensorFlowOptions = tensorFlowOptions.Value;
        this.taskOptions = taskOptions.Value;
        this.factory = factory;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }
    public string PredictImage(InMemoryImage image, string imageSetPath)
    {
        var outputFolderPath = PathUtils.GetPath(tensorFlowOptions.OutputFilePath, imageSetPath);
        var options = new ExtendedTaskOptions()
        {
            InputFolderPath = PathUtils.GetPath(tensorFlowOptions.InputFilePath, imageSetPath),
            DataDir = PathUtils.GetPath(tensorFlowOptions.TestImagesFilePath, imageSetPath),
            TaskPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath),
            ModelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.ModelName),
            LabelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.LabelFile),
        };
        //load model and use it for prediction
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>((opt) =>
        {
            opt.InputFolderPath = options.InputFolderPath;
            opt.DataDir = options.DataDir;
            opt.TaskPath = options.TaskPath;
            opt.ModelPath = options.ModelPath;
            opt.LabelPath = options.LabelPath;
        });
        var input = ImageUtilByte.ReadImage(image.Image);
        var result = task.Predict(input);
        logger.Information("Prediction on [{set}] Result: {result}", imageSetPath, result);
        return result.Label;
    }

    public void PredictImages(string imageSetPath, ImageLabelMapper? mapper)
    {
        var outputFolderPath = PathUtils.GetPath(tensorFlowOptions.OutputFilePath, imageSetPath);
        var options = new ExtendedTaskOptions()
        {
            InputFolderPath = PathUtils.GetPath(tensorFlowOptions.InputFilePath, imageSetPath),
            DataDir = PathUtils.GetPath(tensorFlowOptions.TestImagesFilePath, imageSetPath),
            TaskPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath),
            ModelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.ModelName),
            LabelPath = Path.Combine(outputFolderPath, taskOptions.ClassificationModelPath, taskOptions.LabelFile),
        };

        //load model and use it for prediction
        var task = factory.AddImageClassificationTask<ExtendedTransferLearning>((opt) =>
        {
            opt.InputFolderPath = options.InputFolderPath;
            opt.DataDir = options.DataDir;
            opt.TaskPath = options.TaskPath;
            opt.ModelPath = options.ModelPath;
            opt.LabelPath = options.LabelPath;
            opt.Mapper = mapper;
        });

        var images = imageLoader.LoadImagesMappedToLabelCategory(options.DataDir, options.InputFolderPath!, options.Mapper).ToList();
        foreach (var imageData in images)
        {
            var input = ImageUtil.ReadImageFromFile(imageData.FullFileName());
            var result = task.Predict(input);
            logger.Information("Prediction on [{set}] {image} Result: {result}", imageSetPath, imageData.ImagePath, result);
        }
    }

    public class ImageUtilByte
    {
        public static Tensor ReadImage(byte[] image, int input_height = 299, int input_width = 299, int channels = 3, int input_mean = 0, int input_std = 255)
        {
            tf.enable_eager_execution();

            //look into png etc.

            Tensor file_reader = new Tensor(image, shape: Shape.Scalar);
            var image_reader = tf.image.decode_jpeg(file_reader, channels: channels, name: "jpeg_reader");

            var caster = tf.cast(image_reader, tf.float32);
            var dims_expander = tf.expand_dims(caster, 0);
            var resize = tf.constant(new int[] { input_height, input_width });
            var bilinear = tf.image.resize_bilinear(dims_expander, resize);
            var sub = tf.subtract(bilinear, new float[] { input_mean });
            var normalized = tf.divide(sub, new float[] { input_std });
            tf.Context.restore_mode();
            return normalized;
        }
    }

}


