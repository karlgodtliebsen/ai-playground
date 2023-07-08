namespace AI.CaaP.Domain;

public enum ConversationRole
{
    System,
    User,
    Assistant,
    Function
}

public static class ConversationRoleExtensions
{
    public static string ToRole(this ConversationRole role)
    {
        return role.ToString().ToLower();
    }
}
