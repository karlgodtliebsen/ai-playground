using System.Text.Json.Serialization;

using OpenAI.Client.Domain;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class ModelBaseRequest : IModelRequest
{
    [JsonIgnore]
    public string RequestUri { get; set; }


    [JsonPropertyName("model")]
    public string Model { get; set; }
}
