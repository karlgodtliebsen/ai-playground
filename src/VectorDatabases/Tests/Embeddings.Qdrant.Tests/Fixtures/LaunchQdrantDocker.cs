using AI.Test.Support;

namespace Embeddings.Qdrant.Tests.Fixtures;

public static class LaunchQdrantDocker
{
    //docker desktop settings /temp/qdrant_storage     /qdrant/storage  63333 6334
    //ensure singleton 
    static LaunchQdrantDocker()
    {
        new DockerLauncher().Launch();
    }

    public static void Launch()
    {
        //no action
    }
}
