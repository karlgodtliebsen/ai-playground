namespace LLamaSharpApp.WebAPI.Models.Requests;

/// <summary>
/// DeTokenizeMessageRequest
/// </summary>
public class DeTokenizeMessageRequest
{

    public int[] Tokens { get; set; }

    public bool UsePersistedModelState { get; set; } = true;
}
