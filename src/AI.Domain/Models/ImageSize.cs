namespace AI.Domain.Models;

/// <summary>
/// Represents available sizes for image generation endpoints
/// </summary>
public class ImageSize
{
    private ImageSize(string value)
    {
        Value = value;
    }

    private string Value { get; set; }

    /// <summary>
    /// Requests an image that is 256x256
    /// </summary>
    public static ImageSize Size256 => new ImageSize("256x256");

    /// <summary>
    /// Requests an image that is 512x512
    /// </summary>
    public static ImageSize Size512 => new ImageSize("512x512");

    /// <summary>
    /// Requests and image that is 1024x1024
    /// </summary>
    public static ImageSize Size1024 => new ImageSize("1024x1024");

    /// <summary>
    /// Gets the string value for this size to pass to the API
    /// </summary>
    /// <returns>The size as a string</returns>
    public override string ToString()
    {
        return Value;
    }


}