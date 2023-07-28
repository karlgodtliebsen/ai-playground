using ImageClassification.Domain.Configuration;

using Microsoft.Extensions.DependencyInjection;

using SciSharp.Models;

namespace ImageClassification.Domain.TransferLearning;

public class ExtendedModelFactory
{
    private readonly IServiceProvider sp;
    private readonly ModelContext context;

    public ModelContext Context => context;

    public ExtendedModelFactory(IServiceProvider sp)
    {
        this.sp = sp;
        context = new ModelContext();
    }

    public IImageClassificationTask AddImageClassificationTask<T>(Action<ExtendedTaskOptions>? options) where T : IImageClassificationExtendedTask
    {
        var task = sp.GetRequiredService<T>();
        task.Configure(options);
        context.ImageClassificationTask = task;
        return context.ImageClassificationTask;
    }
}
