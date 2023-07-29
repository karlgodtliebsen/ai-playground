namespace OpenAI.Client.Domain;

/// <summary>
///  <a href="https://platform.openai.com/docs/models/how-we-use-your-data" />
/// <a href="https://learn.microsoft.com/en-us/semantic-kernel/prompt-engineering/llm-models?source=recommendations" />
/// </summary>
public static class OpenAiModelMap
{

    public static Dictionary<string, string[]> Map { get; set; } = new();

    /// <summary>
    /// <a href="https://platform.openai.com/docs/models/how-we-use-your-data" />
    /// <a href="https://learn.microsoft.com/en-us/semantic-kernel/prompt-engineering/llm-models?source=recommendations" />
    /// /// </summary>
    static OpenAiModelMap()
    {
        Map.Add("chat/completions", "gpt-4, gpt-3.5-turbo".Split(','));
        Map.Add("completions", "text-davinci-003, text-davinci-002, text-curie-001, text-babbage-001, text-ada-001".Split(','));
        Map.Add("edits", "text-davinci-edit-001, code-davinci-edit-001".Split(','));
        Map.Add("audio/transcriptions", "whisper-1".Split(','));
        Map.Add("audio/translations", "whisper-1".Split(','));
        Map.Add("fine-tunes", "davinci, curie, babbage, ada".Split(','));
        Map.Add("embeddings", "text-embedding-ada-002, text-search-ada-doc-001".Split(','));
        Map.Add("moderations", "text-moderation-stable, text-moderation-latest".Split(','));
    }
}
