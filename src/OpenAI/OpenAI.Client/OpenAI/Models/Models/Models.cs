using System.Text.Json.Serialization;

namespace OpenAI.Client.OpenAI.Models.Models;

public class Models
{
    [JsonPropertyName("data")]
    public Model[] ModelData { get; set; }

}