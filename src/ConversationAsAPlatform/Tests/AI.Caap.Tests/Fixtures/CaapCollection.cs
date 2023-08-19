
namespace AI.Caap.Tests.Fixtures;

[CollectionDefinition("Caap Collection")]
public class CaapCollection : ICollectionFixture<CaapWithDatabaseTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[CollectionDefinition("Caap InMemory Collection")]
public class CaapInMemoryCollection : ICollectionFixture<CaapWithInMemoryDatabaseTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
