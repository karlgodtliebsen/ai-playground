﻿namespace AI.Library.Tests.Support.Tests.QdrantTestHelper;

internal interface IQdrantVectorDbClient
{
    /// <summary>
    /// <a href="https://qdrant.tech/documentation/concepts/collections/">Create collection</a>
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="vectorParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CreateCollection(string collectionName, QdrantVectorDbClient.VectorParams vectorParams, CancellationToken cancellationToken);

    Task<QdrantVectorDbClient.CollectionInfo> GetCollection(string collectionName, CancellationToken cancellationToken);
}
