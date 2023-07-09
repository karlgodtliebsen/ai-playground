namespace Qdrant.Tests.Fixtures;

[CollectionDefinition("VectorDb Collection")]
public class VectorDbCollection : ICollectionFixture<VectorDbTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
