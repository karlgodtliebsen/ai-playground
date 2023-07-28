using ImageClassification.Domain.Configuration;
using SciSharp.Models;

namespace ImageClassification.Domain.TransferLearning;

public interface IImageClassificationExtendedTask : IImageClassificationTask
{
    void Configure(Action<ExtendedTaskOptions>? options);
}
