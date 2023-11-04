using System.Text.Json.Serialization;

namespace Kernel.Memory.NewsFeed.Domain.Util.OpenSearch;

//const string body = """  
//{
//    "settings": {
//        "index": {
//            "number_of_shards": 1,
//            "number_of_replicas": 1
//        }
//    },
//    "mappings": {
//        "properties": {
//            "age": {
//                "type": "integer"
//            }
//        }
//     }
//    }
//""";

public class IndexConfiguration
{
    [JsonPropertyName("settings")]
    public Settings Settings { get; set; } = new Settings();
    [JsonPropertyName("mappings")]
    public Mappings Mappings { get; set; } = new Mappings();
}

public class Settings
{
    [JsonPropertyName("index")]
    public IndexSettings Index { get; set; } = new IndexSettings();

}
public class IndexSettings
{
    [JsonPropertyName("number_of_shards")]
    public int NumberOfShards { get; set; } = 1;
    [JsonPropertyName("number_of_replicas")]
    public int NumberOfReplicas { get; set; } = 1;

}

public class Mappings
{


}
