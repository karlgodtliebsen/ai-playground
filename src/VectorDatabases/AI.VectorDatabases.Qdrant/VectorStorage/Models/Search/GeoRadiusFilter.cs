﻿using System.Text.Json.Serialization;

namespace AI.VectorDatabase.Qdrant.VectorStorage.Models.Search;

public class GeoRadiusFilter
{
    [JsonPropertyName("center")] public TBRFilter? Center { get; set; } = default!;
    [JsonPropertyName("radius")] public float Radius { get; set; }
}