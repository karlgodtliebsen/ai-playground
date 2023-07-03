using System.Text.Json.Serialization;

using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.Models.Images;

namespace OpenAI.Client.OpenAI.Models.Requests;

public class GenerateVariationsOfImageRequest : IRequest
{
    [JsonIgnore]
    public string RequestUri { get; set; } = "images/variations";


    [JsonIgnore]
    /// <summary>
    /// Contain the image to create variations of.
    /// </summary>
    public Stream ImageStream { get; init; }

    /// <summary>
    /// The image to edit.
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; init; }


    /// <summary>
    /// The number of images to generate. Must be between 1 and 10.
    /// </summary>
    [JsonPropertyName("n")]
    public int NumberOfImagesToGenerate { get; init; } = 1;

    /// <summary>
    /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
    /// Use ImageSize to set this property
    /// </summary>
    [JsonPropertyName("size")]
    public string Size => ImageSize.ToString();

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
