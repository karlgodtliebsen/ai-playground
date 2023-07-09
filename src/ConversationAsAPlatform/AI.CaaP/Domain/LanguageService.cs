using OpenAI.Client.OpenAI.Models.Chat;

namespace AI.CaaP.Domain;

public class LanguageService : ILanguageService
{

    /// <summary>
    /// System prompt to send with user prompts to instruct the model for chat session
    /// </summary>
    private readonly string systemPrompt = @"
        You are an AI assistant that helps people find information.
        Provide concise answers that are polite and professional." + Environment.NewLine;

    /// <summary>    
    /// System prompt to send with user prompts to instruct the model for summarization
    /// </summary>
    private readonly string summarizePrompt = @"Summarize this prompt in one or two words to use as a label in a button on a web page" + Environment.NewLine;

    public Conversation GetStandardSystemPrompt()
    {
        return new Conversation
        {
            Role = ChatMessageRole.System.AsOpenAIRole(),
            Content = systemPrompt.Trim()
        };
    }

    public Conversation GetSummarizePrompt()
    {
        return new Conversation
        {
            Role = ChatMessageRole.System.AsOpenAIRole(),
            Content = summarizePrompt.Trim()
        };
    }

    public Conversation? GetPromptByIndex(int @index)
    {
        switch (index)
        {
            case 1:
                return GetStandardSystemPrompt();
            case 12:
                return GetSummarizePrompt();
        }
        return default!;
    }

}
