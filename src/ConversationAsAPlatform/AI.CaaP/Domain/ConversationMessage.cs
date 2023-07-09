namespace AI.CaaP.Domain;

public class ConversationMessage
{
    public IList<Conversation> Conversations { get; init; }
    public Guid UserId { get; init; }
    public Guid? AgentId { get; set; }
    public bool AddSystemPrompt { get; set; } = true;
    public int SystemPrompt { get; set; } = 1;
}
