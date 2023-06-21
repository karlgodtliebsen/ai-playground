﻿using Microsoft.Extensions.Options;

namespace OpenAI.Client.Domain;

public class ModelRequestFactory : IModelRequestFactory
{
    private readonly OpenAiModelsVerification modelVerificationOptions;

    public ModelRequestFactory(IOptions<OpenAiModelsVerification> modelOptions)
    {
        this.modelVerificationOptions = modelOptions.Value;
    }
    /// <summary>
    /// Creates a request for the specified model
    /// </summary>
    /// <typeparam name="T">The Request to Create an Instance of</typeparam>
    /// <param name="create"></param>/// <returns></returns>
    /// <remarks>Details at https://platform.openai.com/docs/models/how-we-use-your-data</remarks>
    /// <exception cref="ArgumentException"></exception>
    public T CreateRequest<T>(Func<T> create) where T : class, IModelRequest, new()
    {

        var model = create();
        if (!modelVerificationOptions.Verify(model))
        {
            throw new ArgumentException($"Model {model.Model} is not supported for {model.RequestUri}");
        }

        return model;
    }

    public IList<string> GetModels(string requestUri)
    {
        if (OpenAiModeMap.Map.TryGetValue(requestUri, out var models))
        {
            return models;
        }
        return new List<string>();

    }

}


