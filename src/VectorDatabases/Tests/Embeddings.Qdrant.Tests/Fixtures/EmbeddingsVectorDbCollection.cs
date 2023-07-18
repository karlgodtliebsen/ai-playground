namespace Embeddings.Qdrant.Tests.Fixtures;

[CollectionDefinition("EmbeddingsAndVectorDb Collection")]
public class EmbeddingsVectorDbCollection : ICollectionFixture<EmbeddingsVectorDbTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
