﻿using System.Text.Json.Serialization;

using LLamaSharp.Domain.Domain.Models;

namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

/// <summary>
/// Request object to hold the text/message to be sent to the executor
/// </summary>
public class ExecutorInferRequest : TextMessageRequest
{
    /// <summary>
    /// Request specific options
    /// </summary>
    public InferenceRequestResponse? InferenceOptions { get; set; } = default!;

    /// <summary>
    /// Discriminator for the statefull Executor type
    /// Will be ignored when UseStatelessExecutor is true
    /// May be one of the following:InteractiveExecutor or InstructExecutor 
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InstructModeExecute.md"/>
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/docs/Examples/InteractiveModeExecute.md"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InferenceType InferenceType { get; set; } = InferenceType.InteractiveExecutor;


    /// <summary>
    /// When true, the models state will be loaded and saved from the file system
    /// </summary>
    public bool? UsePersistedModelState { get; set; } = true;
    /// <summary>
    /// When true, the Executor state will be loaded and saved from the file system
    /// </summary>
    public bool? UsePersistedExecutorState { get; set; } = true;

    /// <summary>
    /// When true, the a Stateless Executor will be used
    /// </summary>
    public bool? UseStatelessExecutor { get; set; } = false;

    /// <summary>
    /// Use the systems build in AntiPrompt
    /// </summary>
    public bool UseDefaultAntiPrompt { get; set; } = false!;

    /// <summary>
    /// Use the systems build in Prompt
    /// </summary>
    public bool UseDefaultPrompt { get; set; } = false;
}



