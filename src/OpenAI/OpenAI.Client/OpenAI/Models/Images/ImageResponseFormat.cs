namespace OpenAI.Client.OpenAI.Models.Images;

/// <summary>
/// Represents available response formats for image generation endpoints
/// </summary>
public class ImageResponseFormat
{
    private ImageResponseFormat(string value)
    {
        Value = value;
    }

    private string Value { get; set; }

    /// <summary>
    /// Requests an image that is 256x256
    /// </summary>
    public static ImageResponseFormat Url => new ImageResponseFormat("url");

    /// <summary>
    /// Requests an image that is 512x512
    /// </summary>
    public static ImageResponseFormat B64Json => new ImageResponseFormat("b64_json");


    /// <summary>
    /// Gets the string value for this response format to pass to the API
    /// </summary>
    /// <returns>The response format as a string</returns>
    public override string ToString()
    {
        return Value;
    }

}
