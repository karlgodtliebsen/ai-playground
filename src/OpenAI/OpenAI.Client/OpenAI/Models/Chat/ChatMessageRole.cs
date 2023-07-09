namespace OpenAI.Client.OpenAI.Models.Chat;


public enum ChatMessageRole
{
    System,
    User,
    Assistant,
    Function,
}


public static class ChatMessageRoleExtensions
{
    public static string AsOpenAIRole(this ChatMessageRole role)
    {
        return role.ToString().ToLower();
    }
}
