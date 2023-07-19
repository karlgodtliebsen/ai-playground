using AI.VectorDatabase.Qdrant.VectorStorage.Models;

namespace AI.VectorDatabase.Qdrant.Configuration;

public class QdrantOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "Qdrant";

    public string Url { get; set; }

    public int? Port { get; set; } = default;

    public string ApiKey { get; set; } = "n/a";


    public int DefaultDimension { get; set; } = 4;

    public string DefaultDistance { get; set; } = Distance.COSINE;

    public bool DefaultStoreOnDisk { get; set; } = false;

}
