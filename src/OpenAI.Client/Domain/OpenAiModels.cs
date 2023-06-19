﻿namespace OpenAI.Client.Domain;

public class OpenAiModels
{
    public bool Verify(IModelRequest request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.RequestUri);
        ArgumentException.ThrowIfNullOrEmpty(request.Model);

        if (OpenAiModeMap.Map.TryGetValue(request.RequestUri, out var models))
        {
            var result = models.Any(m => m.Trim() == request.Model.Trim());
            return result;
        }

        return false;
    }
}