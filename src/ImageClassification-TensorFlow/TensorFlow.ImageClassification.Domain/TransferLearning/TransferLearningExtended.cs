using ImageClassification.Domain.Configuration;
using ImageClassification.Domain.Utils;

using Microsoft.Extensions.Options;

using SciSharp.Models;

using Serilog;

using Tensorflow.Keras.Utils;

namespace ImageClassification.Domain.TransferLearning
{
    /// <summary>
    /// In this tutorial, we will reuse the feature extraction capabilities from powerful image classifiers trained on ImageNet 
    /// and simply train a new classification layer on top. Transfer learning is a technique that shortcuts much of this 
    /// by taking a piece of a model that has already been trained on a related task and reusing it in a new model.
    /// 
    /// https://www.tensorflow.org/hub/tutorials/image_retraining
    /// </summary>
    public partial class TransferLearningExtended : IImageClassificationExtendedTask
    {
        private readonly IImageLoader imageLoader;
        private readonly ILogger logger;
        private string taskDir = null!;
        private string summariesDir = null!;
        private string bottleneckDir = null!;
        private readonly bool isImportingGraph = true;
        private readonly ExtendedTaskOptions options;
        private string[]? labels = null!;

        public TransferLearningExtended(IImageLoader imageLoader, IOptions<ExtendedTaskOptions> injectedOptions, ILogger logger)
        {
            this.imageLoader = imageLoader;
            this.logger = logger;
            this.options = injectedOptions.Value;
        }

        public void Config(TaskOptions options)
        {
        }

        public void Configure(Action<ExtendedTaskOptions>? injectOptions)
        {
            options.ModelPath = Path.Combine(options.TaskPath, options.ModelName!);
            options.LabelPath = Path.Combine(options.TaskPath, options.LabelFile!);
            injectOptions?.Invoke(options);
            taskDir = options.TaskPath;
            if (!Directory.Exists(taskDir))
            {
                Directory.CreateDirectory(taskDir);
            }

            // download graph meta data
            if (!File.Exists(Path.Combine(taskDir, options.MetaDataPath, options.MetaDataFilename)))
            {
                Web.Download(options.MetaDataUrl, Path.Combine(taskDir, options.MetaDataPath), options.MetaDataFilename);
            }

            if (!File.Exists(Path.Combine(taskDir, options.TfModulesZipFile)))
            {
                Web.Download(options.CheckpointDataUrl, taskDir, options.TfModulesZipFile);
                Compress.UnZip(Path.Combine(taskDir, options.TfModulesZipFile), Path.Combine(taskDir, options.TfModules));
            }

            // Prepare necessary directories that can be used during training
            summariesDir = Path.Combine(taskDir, options.RetrainLogs);
            Directory.CreateDirectory(summariesDir);
            bottleneckDir = Path.Combine(taskDir, options.Bottleneck);
            Directory.CreateDirectory(bottleneckDir);
        }

        public void SetModelArgs<T>(T args)
        {
        }
    }
}
