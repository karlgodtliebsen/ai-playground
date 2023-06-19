
using OpenAI.Client.Models.Requests;

using System.Text.Json.Serialization;

namespace OpenAI.Client.Models;

public class FineTunes
{

    /// <summary> Id for  response. </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary> Object for  response. </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";


    [JsonPropertyName("data")]

    public FineTuneRequest[] Data { get; set; }

}