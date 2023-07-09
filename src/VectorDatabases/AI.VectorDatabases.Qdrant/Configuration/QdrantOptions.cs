using AI.VectorDatabaseQdrant.VectorStorage.Models;

namespace AI.VectorDatabaseQdrant.Configuration;

public class QdrantOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string ConfigSectionName = "Qdrant";

    public string Url { get; set; }

    public string ApiKey { get; set; } = "n/a";


    public int DefaultDimension { get; set; } = 4;

    public string DefaultDistance { get; set; } = Distance.COSINE;

    public bool DefaultStoreOnDisk { get; set; } = false;

}
