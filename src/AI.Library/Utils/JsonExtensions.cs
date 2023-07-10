namespace AI.Library.Utils
{
    public static class JsonExtensions
    {

        public static string ToJson(this object obj, JsonSerializerOptions? serializerOptions = null)
        {
            if (serializerOptions is not null)
            {
                return JsonSerializer.Serialize(obj, serializerOptions!);
            }
            return JsonSerializer.Serialize(obj);
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
