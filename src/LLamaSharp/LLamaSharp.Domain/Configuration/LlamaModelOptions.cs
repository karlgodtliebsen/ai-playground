﻿using LLama.Common;

namespace LLamaSharp.Domain.Configuration;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public sealed record LLamaModelOptions : ModelParams
{
    const string DefaultFileExtension = ".gguf";
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlamaModel";

    /// <summary>
    /// Constructor for LlamaModelOptions with default ModelPath
    /// </summary>
    public LLamaModelOptions() : base("./LlamaModels") { }

    public string GetSanitizeSensitiveData()
    {
        var path = Path.GetFullPath(ModelPath);
        return path.Split(Path.DirectorySeparatorChar).LastOrDefault()!;
    }

    public void RestoreSanitizedSensitiveData(string? modelName)
    {
        if (modelName is null) return;
        if (!modelName.EndsWith(DefaultFileExtension))
        {
            modelName = $"{modelName}{DefaultFileExtension}";
        }
        if (ModelPath!.EndsWith(modelName))
        {
            return;
        }

        var path = Path.GetFullPath(ModelPath);
        var r = path.Split(Path.DirectorySeparatorChar).LastOrDefault()!;
        ModelPath = path.Replace(r, modelName);
    }
}
