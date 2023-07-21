namespace SemanticKernel.Tests.Fixtures;

[CollectionDefinition("SemanticKernel Collection")]
public class SemanticKernelCollection : ICollectionFixture<SemanticKernelTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[CollectionDefinition("SemanticKernel Base Collection")]
public class SemanticKernelBaseCollection : ICollectionFixture<SemanticKernelTestFixtureBase>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
