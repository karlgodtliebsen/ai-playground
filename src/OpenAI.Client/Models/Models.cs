using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class Models
{
    [JsonPropertyName("data")]
    public Model[] ModelData { get; set; }

}