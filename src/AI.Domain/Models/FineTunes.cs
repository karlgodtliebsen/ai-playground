
using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class FineTunes
{

    /// <summary> Id for  response. </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary> Object for  response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";


    [JsonPropertyName("data")]

    public FineTune[] FineTuneData { get; set; }

}