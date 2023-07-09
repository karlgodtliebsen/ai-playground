namespace AI.CaaP.Domain;

public interface ILanguageService
{
    Conversation GetStandardSystemPrompt();

    Conversation GetSummarizePrompt();

    Conversation? GetPromptByIndex(int @index);
}
