using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

public class PointCollection : List<PointStruct>
{

    public PointCollection(IEnumerable<PointStruct> collection) : base(collection)
    {
    }

    public PointCollection()
    {
    }

    public PointCollection(int capacity) : base(capacity)
    {
    }

    [JsonIgnore]
    public int Dimension
    {
        get
        {
            if (this.Count == 0)
            {
                return 0;
            }
            return this[0].Vector!.Value.Length;
        }
    }
}

