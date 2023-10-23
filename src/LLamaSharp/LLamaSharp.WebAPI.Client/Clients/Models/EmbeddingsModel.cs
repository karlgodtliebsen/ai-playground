namespace LLamaSharp.WebAPI.Client.Clients.Models;

/// <summary>
/// Request object to hold the text to be sent to the embedding algoritm
/// </summary>
public class EmbeddingsModel : TextMessageModel
{
    /// <summary>
    /// Use Stateful Model
    /// </summary>
    public bool UsePersistedModelState { get; set; } = false;

    //possible dangerous to expose. Must be strengthened
    public bool AddBos { get; set; } = true!;
    public int Threads { get; set; } = -1;
    public string Encoding { get; set; } = "UTF-8";
}
