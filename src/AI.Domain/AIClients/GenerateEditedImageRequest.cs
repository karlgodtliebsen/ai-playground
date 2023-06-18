using System.Text.Json.Serialization;

namespace AI.Domain.AIClients;

public class GenerateEditedImageRequest
{
    [JsonIgnore]
    public Stream ImageStream { get; init; }

    /// <summary>
    /// The image to edit. Must be a valid PNG file, less than 4MB, and square.
    /// If mask is not provided, image must have transparency, which will be used as the mask.
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; init; }

    /// <summary>
    /// A text description of the desired image(s). The maximum length is 1000 characters.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; }


    [JsonIgnore]
    public Stream? MaskStream { get; init; }

    /// <summary>
    /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited.
    /// Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
    /// </summary>
    [JsonPropertyName("mask")]
    public string? Mask { get; init; } = default!;


    /// <summary>
    /// The number of images to generate. Must be between 1 and 10.
    /// </summary>
    [JsonPropertyName("n")]
    public int NumberOfImagesToGenerate { get; init; } = 1!;

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