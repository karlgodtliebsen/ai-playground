using System.Text.Json.Serialization;

namespace AI.VectorDatabaseQdrant.VectorStorage.Models.Payload;

public class UpdateResult
{
    [JsonPropertyName("operation_id")] public int OperationId { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; }
}