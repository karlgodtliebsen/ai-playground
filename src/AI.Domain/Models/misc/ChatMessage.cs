//using System.Text.Json.Serialization;

//namespace Kwanify.Xbrl.AI.Domain.Tests.Models;

//public class ChatMessage
//{
//    /// <summary>
//    /// Creates an empty <see cref="ChatMessage"/>, with <see cref="Role"/> defaulting to <see cref="ChatMessageRole.User"/>
//    /// </summary>
//    public ChatMessage()
//    {
//        Role = ChatMessageRole.User;
//    }

//    /// <summary>
//    /// Constructor for a new Chat Message
//    /// </summary>
//    /// <param name="role">The role of the message, which can be "system", "assistant" or "user"</param>
//    /// <param name="content">The text to send in the message</param>
//    public ChatMessage(ChatMessageRole role, string content)
//    {
//        Role = role;
//        Content = content;
//    }

//    [JsonPropertyName("role")]
//    internal string rawRole { get; set; }

//    /// <summary>
//    /// The role of the message, which can be "system", "assistant" or "user"
//    /// </summary>
//    [JsonIgnore]
//    public ChatMessageRole Role
//    {
//        get
//        {
//            return ChatMessageRole.FromString(rawRole);
//        }
//        set
//        {
//            rawRole = value.ToString();
//        }
//    }

//    /// <summary>
//    /// The content of the message
//    /// </summary>
//    [JsonPropertyName("content")]
//    public string Content { get; set; }

//    /// <summary>
//    /// An optional name of the user in a multi-user chat 
//    /// </summary>
//    [JsonPropertyName("name")]
//    public string Name { get; set; }
//}