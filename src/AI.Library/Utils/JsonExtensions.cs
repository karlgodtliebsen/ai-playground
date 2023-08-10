using System.Text.Json.Serialization;

namespace AI.Library.Utils
{
    /// <summary>
    /// Json Extensions
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        /// <summary>
        /// Serializes the object to a json string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializerOptions"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, JsonSerializerOptions? serializerOptions = null)
        {
            serializerOptions ??= DefaultJsonSerializerOptions;
            return JsonSerializer.Serialize(obj, serializerOptions!);
        }

        /// <summary>
        /// Serializes the object to a json file
        /// </summary>
        /// <param name="options"></param>
        /// <param name="fileInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="serializerOptions"></param>
        /// <returns></returns>
        public static async Task ToJsonFile(this object options, FileInfo fileInfo, CancellationToken cancellationToken, JsonSerializerOptions? serializerOptions = null)
        {
            serializerOptions ??= DefaultJsonSerializerOptions;
            if (File.Exists(fileInfo.FullName)) File.Delete(fileInfo.FullName); //Can be improved to make it more resilient by creating a copy and so on
            await using var stream = File.OpenWrite(fileInfo.FullName);
            await JsonSerializer.SerializeAsync(stream, options, serializerOptions, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Deserializes the json file to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="serializerOptions"></param>
        /// <returns></returns>
        public static async Task<T> FromJsonFile<T>(this FileInfo fileInfo, CancellationToken cancellationToken, JsonSerializerOptions? serializerOptions = null)
        {
            serializerOptions ??= DefaultJsonSerializerOptions;
            await using var stream = File.OpenRead(fileInfo.FullName);
            var options = await JsonSerializer.DeserializeAsync<T>(stream, serializerOptions, cancellationToken: cancellationToken);
            return options!;
        }

        /// <summary>
        /// Deserializes the json string to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">json formatted string</param>
        /// <param name="serializerOptions"></param>
        /// <returns></returns>
        public static T? FromJson<T>(this string json, JsonSerializerOptions? serializerOptions = null)
        {
            serializerOptions ??= DefaultJsonSerializerOptions;
            return JsonSerializer.Deserialize<T>(json, serializerOptions);
        }
    }
}
