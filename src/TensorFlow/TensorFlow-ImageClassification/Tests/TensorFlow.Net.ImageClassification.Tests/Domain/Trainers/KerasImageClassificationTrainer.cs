using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Configuration;
using ML.Net.ImageClassification.Tests.Domain.Models;

using Serilog;

using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.Engine;

using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace ML.Net.ImageClassification.Tests.Domain.Trainers;

//Based on:
//https://github.com/SciSharp/SciSharp-Stack-Examples/blob/2aeef9eff0b48148732aa851cdeecf41f23534b7/src/TensorFlowNET.Examples/ImageProcessing/ImageClassificationKeras.cs

public sealed class KerasImageClassificationTrainer : IKerasTrainer
{
    private readonly IImageLoader imageLoader;
    private readonly ILogger logger;

    private readonly TensorFlowImageClassificationOptions options;
    public KerasImageClassificationTrainer(IOptions<TensorFlowImageClassificationOptions> options, IImageLoader imageLoader, ILogger logger)
    {
        this.options = options.Value;
        this.imageLoader = imageLoader;
        this.logger = logger;
    }

    int batch_size = 32;
    int epochs = 10;
    Shape img_dim = (64, 64);
    IDatasetV2 train_ds, val_ds;
    Model model;

    public string TrainModel(string imageSetPath, ImageLabelMapper? mapper)
    {

        tf.enable_eager_execution();
        var imageSetFolderPath = PathUtils.GetPath(options.TrainImagesFilePath, imageSetPath);
        PrepareData(imageSetFolderPath);
        BuildModel();
        Train();

        return "ok";
    }

    public void BuildModel()
    {
        int num_classes = 5;
        // var normalization_layer = tf.keras.layers.Rescaling(1.0f / 255);
        var layers = keras.layers;
        var myLayers = new List<ILayer>
        {

            layers.Rescaling(1.0f / 255, input_shape: (img_dim.dims[0], img_dim.dims[1], 3)),
            layers.Conv2D(16, 3, padding: "same", activation: keras.activations.Relu),
            layers.MaxPooling2D(),
            /*layers.Conv2D(32, 3, padding: "same", activation: "relu"),
            layers.MaxPooling2D(),
            layers.Conv2D(64, 3, padding: "same", activation: "relu"),
            layers.MaxPooling2D(),*/
            layers.Flatten(),
            layers.Dense(128, activation: keras.activations.Relu),
            layers.Dense(num_classes)
        };
        model = keras.Sequential(myLayers);

        model.compile(optimizer: keras.optimizers.Adam(),
            loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            metrics: new[] { "accuracy" });

        model.summary();
    }

    public void Train()
    {
        model.fit(train_ds, validation_data: val_ds, epochs: epochs);
    }


    private void PrepareData(string imageSetFolderPath)
    {
        train_ds = keras.preprocessing.image_dataset_from_directory(imageSetFolderPath,
            validation_split: 0.2f,
            subset: "training",
            seed: 123,
            image_size: img_dim,
            batch_size: batch_size);

        val_ds = keras.preprocessing.image_dataset_from_directory(imageSetFolderPath,
            validation_split: 0.2f,
            subset: "validation",
            seed: 123,
            image_size: img_dim,
            batch_size: batch_size);

        train_ds = train_ds.shuffle(1000).prefetch(buffer_size: -1);
        val_ds = val_ds.prefetch(buffer_size: -1);
    }
}
