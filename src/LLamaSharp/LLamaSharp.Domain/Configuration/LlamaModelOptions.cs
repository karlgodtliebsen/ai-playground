﻿using LLama.Common;

namespace LLamaSharp.Domain.Configuration;
/// <summary>
/// Holds the user applicable settings for the LLamaSharpApp.WebAPI
/// </summary>
public class LlamaModelOptions : ModelParams
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "LlamaModel";

    /// <summary>
    /// Constructor for LlamaModelOptions with default ModelPath
    /// </summary>
    public LlamaModelOptions() : base("./LlamaModels") { }

    public string GetSanitizeSensitiveData()
    {
        var path = Path.GetFullPath(ModelPath);
        return path.Split(Path.DirectorySeparatorChar).LastOrDefault()!;
    }

    public void RestoreSanitizedSensitiveData(string? modelName)
    {
        if (modelName is null || ModelPath!.EndsWith(modelName))
        {
            return;
        }
        var path = Path.GetFullPath(ModelPath);
        var r = path.Split(Path.DirectorySeparatorChar).LastOrDefault()!;
        ModelPath = path.Replace(r, modelName);
    }


    /// <summary>
    /// 
    /// </summary>
    public string[]? AntiPrompt { get; set; }

    /// <summary>
    /// Path to the promptFile
    /// </summary>
    public string? Prompt { get; set; }


}