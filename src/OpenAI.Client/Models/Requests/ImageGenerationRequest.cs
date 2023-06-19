using System.Text.Json.Serialization;
using OpenAI.Client.Domain;

namespace OpenAI.Client.Models.Requests;

/// <summary>
/// Ask the API to Creates an image given a prompt.
/// </summary>
/// <param name="request">Request to be send</param>
/// <returns>Asynchronously returns the image result. Look in its <see cref="Data.Url"/> </returns>
public class ImageGenerationRequest : IRequest
{

    public ImageGenerationRequest()
    {
        RequestUri = "images/generations";
    }

    [JsonIgnore]
    public string RequestUri { get; set; }


    /// <summary>
    /// A text description of the desired image(s). The maximum length is 1000 characters.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; }

    /// <summary>
    /// The number of images to generate. Must be between 1 and 10.
    /// </summary>
    [JsonPropertyName("n")]
    public int? NumberOfImagesToGenerate { get; init; } = default!;

    /// <summary>
    /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
    /// </summary>
    [JsonPropertyName("size")]
    public string? Size => ImageSize.ToString();

    /// <summary>
    /// Types ImageSize setting
    /// </summary>
    [JsonIgnore]
    public ImageSize ImageSize { get; init; } = ImageSize.Size1024;


    /// <summary>
    /// The format in which the generated images are returned. Must be one of url or b64_json
    /// Use ImageResponseFormat to set this property
    /// </summary>
    [JsonPropertyName("response_format")]
    public string ResponseFormat => ImageResponseFormat.ToString();


    /// <summary>
    /// The format in which the generated images are returned. Must be one of url or b64_json
    /// </summary>
    [JsonIgnore]
    public ImageResponseFormat ImageResponseFormat { get; init; } = ImageResponseFormat.Url;

    /// <summary>
    /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; init; } = default!;

}