using AI.Test.Support;

using DotNet.Testcontainers.Configurations;

namespace Embeddings.Qdrant.Tests.Fixtures;

public static class LaunchDocker
{
    //static LaunchDocker()
    //{
    //   TestContainerFactory.Build("qdrant/qdrant:latest", 6333, 6333, 6333, (b) => b.WithVolumeMount("./qdrant_storage:", "/qdrant/storage")).Wait();
    //}
    static LaunchDocker()
    {
        //https://www.lambdatest.com/automation-testing-advisor/csharp/methods/DotNet.Testcontainers.Builders.TestcontainersBuilderTDockerContainer.WithVolumeMount
        var sourcePath = Path.GetFullPath(Path.Combine("/temp", "qdrant_storage"));
        if (!Directory.Exists(sourcePath))
        {
            Directory.CreateDirectory(sourcePath);
        }
        TestContainerFactory.Build("qdrant/qdrant:latest", 6333, 6333, 6333,
            b => b.WithBindMount(sourcePath, "/qdrant/storage", AccessMode.ReadWrite)
        ).Wait();
    }

    public static void Launch()
    {
        //no action
    }

    /*
     https://github.com/qdrant/qdrant/blob/master/QUICK_START.md
    docker run -p 6333:6333 qdrant/qdrant


    docker run -p 6333:6333 -v ./qdrant_storage:/qdrant/storage qdrant/qdrant:latest

    docker run -p 6333:6333 \
    -v $(pwd)/path/to/data:/qdrant/storage \
    -v $(pwd)/path/to/custom_config.yaml:/qdrant/config/production.yaml \
    qdrant/qdrant
    */
}
