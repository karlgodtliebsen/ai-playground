using System.Text.Json.Serialization;

namespace AI.Domain.Models;

public class Models
{
    [JsonPropertyName("data")]
    public Model[] ModelData { get; set; }

}