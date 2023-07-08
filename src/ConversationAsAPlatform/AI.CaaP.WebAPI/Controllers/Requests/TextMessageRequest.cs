namespace AI.CaaP.WebAPI.Controllers.Requests;

/// <summary>
/// TextMessageRequest
/// </summary>
public class TextMessageRequest : BaseRequest
{
    /// <summary>
    /// Prompt/Chat text
    /// </summary>
    public string? Text { get; set; }
}
