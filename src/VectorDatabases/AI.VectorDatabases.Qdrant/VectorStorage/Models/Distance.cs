using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models;

public class Distance
{
    public const string COSINE = "Cosine";
    public const string EUCLID = "Euclid";
    public const string DOT = "Dot";
}


/// <summary>
/// The vector distance type used by Qdrant.
/// <a href="https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Connectors/Connectors.Memory.Qdrant/QdrantDistanceType.cs" />
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QdrantDistanceType
{
    /// <summary>
    /// Cosine of the angle between vectors, aka dot product scaled by magnitude. Cares only about angle difference.
    /// </summary>
    Cosine,

    /// <summary>
    /// Consider angle and distance (magnitude) of vectors.
    /// </summary>
    DotProduct,

    /// <summary>
    /// Pythagorean(theorem) distance of 2 multidimensional points.
    /// </summary>
    Euclidean,

    /// <summary>
    /// Sum of absolute differences between points across all the dimensions.
    /// </summary>
    Manhattan,

    /// <summary>
    /// Assuming only the most significant dimension is relevant.
    /// </summary>
    Chebyshev,

    /// <summary>
    /// Generalization of Euclidean and Manhattan.
    /// </summary>
    Minkowski,
}
