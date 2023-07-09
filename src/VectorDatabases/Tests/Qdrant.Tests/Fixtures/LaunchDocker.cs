using AI.Test.Support;

namespace Qdrant.Tests.Fixtures;

public static class LaunchDocker
{
    static LaunchDocker()
    {
        TestContainerFactory.Build("qdrant/qdrant:latest", 6333, 6333, 6333).Wait();
    }

    public static void Launch()
    {
        //no action
    }

    /*
     https://github.com/qdrant/qdrant/blob/master/QUICK_START.md
    docker run -p 6333:6333 qdrant/qdrant

    docker run -p 6333:6333 \
    -v $(pwd)/path/to/data:/qdrant/storage \
    -v $(pwd)/path/to/custom_config.yaml:/qdrant/config/production.yaml \
    qdrant/qdrant
    */
}
