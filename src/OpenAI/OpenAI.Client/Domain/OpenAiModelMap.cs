namespace OpenAI.Client.Domain;

/// <summary>
/// https://platform.openai.com/docs/models/how-we-use-your-data
/// </summary>
public static class OpenAiModelMap
{

    public static Dictionary<string, string[]> Map { get; set; } = new();

    //TODO: should be moved to configuration (appsettings)
    static OpenAiModelMap()
    {
        Map.Add("chat/completions", "gpt-4, gpt-4-0613, gpt-4-32k, gpt-4-32k-0613, gpt-3.5-turbo, gpt-3.5-turbo-0613, gpt-3.5-turbo-16k, gpt-3.5-turbo-16k-0613".Split(','));
        Map.Add("completions", "text-davinci-003, text-davinci-002, text-curie-001, text-babbage-001, text-ada-001".Split(','));
        Map.Add("edits", "text-davinci-edit-001, code-davinci-edit-001".Split(','));
        Map.Add("audio/transcriptions", "whisper-1".Split(','));
        Map.Add("audio/translations", "whisper-1".Split(','));
        Map.Add("fine-tunes", "davinci, curie, babbage, ada".Split(','));
        Map.Add("embeddings", "text-embedding-ada-002, text-search-ada-doc-001".Split(','));
        Map.Add("moderations", "text-moderation-stable, text-moderation-latest".Split(','));
    }
}
