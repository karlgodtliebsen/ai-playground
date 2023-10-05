using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace AI.Library.Utils;

/// <summary>Provides a converter for <see cref="ReadOnlyMemory{Single}"/>.</summary>
public sealed class ReadOnlyMemoryConverter : JsonConverter<ReadOnlyMemory<float>>
{
    /// <summary>An instance of a converter for float[] that all operations delegate to.</summary>
    private static readonly JsonConverter<float[]> s_arrayConverter = (JsonConverter<float[]>)new JsonSerializerOptions().GetConverter(typeof(float[]));

    public override ReadOnlyMemory<float> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        s_arrayConverter.Read(ref reader, typeof(float[]), options).AsMemory();

    public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<float> value, JsonSerializerOptions options) =>
        // This provides an efficient implementation when the ReadOnlyMemory represents the full length of an array.
        // This is the common case for these projects, and thus the implementation doesn't spend more code on a complex
        // implementation to efficiently handle slices or instances backed by MemoryManagers.
        s_arrayConverter.Write(
            writer,
            MemoryMarshal.TryGetArray(value, out ArraySegment<float> array) && array.Count == value.Length ? array.Array! : value.ToArray(),
            options);
}

public sealed class ReadOnlyMemoryConverterArray : JsonConverter<ReadOnlyMemory<float>[]>
{
    /// <summary>An instance of a converter for float[] that all operations delegate to.</summary>
    private static readonly JsonConverter<float[]> s_arrayConverter = (JsonConverter<float[]>)new JsonSerializerOptions().GetConverter(typeof(float[]));

    public override ReadOnlyMemory<float>[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<ReadOnlyMemory<float>> collection = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return collection.ToArray();
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var memory = s_arrayConverter.Read(ref reader, typeof(float[]), options).AsMemory();
                collection.Add(memory);
            }
        }
        return collection.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<float>[] values, JsonSerializerOptions options)
    {
        // This provides an efficient implementation when the ReadOnlyMemory represents the full length of an array.
        // This is the common case for these projects, and thus the implementation doesn't spend more code on a complex
        // implementation to efficiently handle slices or instances backed by MemoryManagers.

        writer.WriteStartArray();
        foreach (var value in values)
        {
            s_arrayConverter.Write(writer, MemoryMarshal.TryGetArray(value, out ArraySegment<float> array) && array.Count == value.Length ? array.Array! : value.ToArray(), options);
        }
        writer.WriteEndArray();
    }
}
