namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Payload;

public class BatchCollection : List<BatchStruct>
{

    public BatchCollection(IEnumerable<BatchStruct> collection) : base(collection)
    {
    }

    public BatchCollection()
    {
    }

    public BatchCollection(int capacity) : base(capacity)
    {
    }

    public int Dimension
    {
        get
        {
            if (this.Count == 0)
            {
                return 0;
            }
            return this[0].Vectors[0].Length;
        }
    }
}
