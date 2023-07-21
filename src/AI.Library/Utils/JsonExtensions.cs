namespace AI.Library.Utils
{
    public static class JsonExtensions
    {

        private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        public static string ToJson(this object obj, JsonSerializerOptions? serializerOptions = null)
        {
            if (serializerOptions is not null)
            {
                return JsonSerializer.Serialize(obj, serializerOptions!);
            }
            return JsonSerializer.Serialize(obj, jsonOptions);
        }

        public static string AsJson(this object obj, JsonSerializerOptions? serializerOptions = null)
        {
            return ToJson(obj, serializerOptions);
        }

        public static T? FromJson<T>(this string json, JsonSerializerOptions? serializerOptions = null)
        {
            if (serializerOptions is not null)
            {
                return JsonSerializer.Deserialize<T>(json, serializerOptions);
            }
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
