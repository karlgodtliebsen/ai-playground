namespace OpenAI.Client.Domain;

public class OpenAiModelsVerification
{
    public bool Verify(IModelRequest request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.RequestUri);
        ArgumentException.ThrowIfNullOrEmpty(request.Model);

        if (OpenAiModelMap.Map.TryGetValue(request.RequestUri, out var models))
        {
            var result = models.Any(m => m.Trim() == request.Model.Trim());
            return result;
        }

        return false;
    }
}
