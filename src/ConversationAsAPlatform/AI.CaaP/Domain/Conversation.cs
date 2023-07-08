using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AI.CaaP.Domain;

public class Conversation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [StringLength(25)]
    public string Role { get; set; } = ConversationRole.User.ToString();

    [StringLength(50)]
    public Guid AgentId { get; set; }

    [StringLength(50)]
    public Guid UserId { get; set; }

    [StringLength(255)]
    public string? Title { get; set; } = default!;

    [StringLength(Int32.MaxValue)]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
}


//https://github.com/Azure-Samples/cosmosdb-chatgpt/blob/main/Models/Message.cs

public record Message
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public string Id { get; set; }

    public string Type { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    public string SessionId { get; set; }

    public DateTime TimeStamp { get; set; }

    public string Sender { get; set; }

    public int? Tokens { get; set; }

    public string Text { get; set; }

    public Message(string sessionId, string sender, int? tokens, string text)
    {
        Id = Guid.NewGuid().ToString();
        Type = nameof(Message);
        SessionId = sessionId;
        Sender = sender;
        Tokens = tokens;
        TimeStamp = DateTime.UtcNow;
        Text = text;
    }
}


public record Session
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public string Id { get; set; }

    public string Type { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    public string SessionId { get; set; }

    public int? TokensUsed { get; set; }

    public string Name { get; set; }

    [JsonIgnore]
    public List<Message> Messages { get; set; }

    public Session()
    {
        Id = Guid.NewGuid().ToString();
        Type = nameof(Session);
        SessionId = this.Id;
        TokensUsed = 0;
        Name = "New Chat";
        Messages = new List<Message>();
    }

    public void AddMessage(Message message)
    {
        Messages.Add(message);
    }

    public void UpdateMessage(Message message)
    {
        var match = Messages.Single(m => m.Id == message.Id);
        var index = Messages.IndexOf(match);
        Messages[index] = message;
    }
}
