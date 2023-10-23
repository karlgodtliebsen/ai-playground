using System.Text.Json.Serialization;

namespace AI.Library.Utils;

/// <summary>
/// Json Extensions
/// </summary>
public static class DefaultJsonSerializerOptions
{
    public static readonly JsonSerializerOptions DefaultOptions = CreateOptions();

    /// <returns></returns>
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new ReadOnlyMemoryConverter(),
                new ReadOnlyMemoryConverterArray()
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };
        return options;
    }
}