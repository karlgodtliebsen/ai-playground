﻿namespace AI.VectorDatabase.Qdrant.VectorStorage;

[Serializable]
public class QdrantException : Exception
{
    public QdrantException(string message) : base(message)
    {
    }
}
