namespace SemanticKernel.Tests.Fixtures;

[CollectionDefinition("SemanticKernel With Docker Collection")]
public class SemanticKernelCollection : ICollectionFixture<SemanticKernelWithDockerTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
