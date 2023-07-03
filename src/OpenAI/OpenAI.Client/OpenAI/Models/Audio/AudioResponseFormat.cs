namespace OpenAI.Client.OpenAI.Models.Audio;

/// <summary>
/// Represents available response formats for image generation endpoints
/// </summary>
public class AudioResponseFormat
{
    private AudioResponseFormat(string value)
    {
        Value = value;
    }

    private string Value { get; set; }

    //json, text, srt, verbose_json, or vtt.
    /// <summary>
    /// Requests audio format
    /// </summary>
    public static AudioResponseFormat Json => new AudioResponseFormat("json");


    /// <summary>
    /// Requests audio format
    /// </summary>
    public static AudioResponseFormat Text => new AudioResponseFormat("text");


    /// <summary>
    /// Requests audio format
    /// </summary>
    public static AudioResponseFormat Srt => new AudioResponseFormat("srt");


    /// <summary>
    /// Requests audio format
    /// </summary>
    public static AudioResponseFormat VerboseJson => new AudioResponseFormat("verbose_json");

    /// <summary>
    /// Requests audio format
    /// </summary>
    public static AudioResponseFormat Vtt => new AudioResponseFormat("vtt");

    /// <summary>
    /// Gets the string value for this response format to pass to the API
    /// </summary>
    /// <returns>The response format as a string</returns>
    public override string ToString()
    {
        return Value;
    }
}
