using LLama.Common;

namespace LLamaSharp.Domain.Configuration;

/// <summary>
/// InferenceOptions
/// </summary>
public sealed record InferenceOptions : InferenceParams
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "Inference";
}
