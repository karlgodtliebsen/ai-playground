namespace AI.CaaP.Configuration;

public class ChunkOptions
{
    /// <summary>
    /// Configuration Section Name
    /// </summary>
    public const string ConfigSectionName = "Chunk";

    /// <summary>
    /// Max chunk character size
    /// </summary>
    public int Size { get; set; } = 256;

    /// <summary>
    /// Overlap word count in between two chunks
    /// </summary>
    public int Conjunction { get; set; } = 5;

    public bool SplitByWord { get; set; } = true;
}
