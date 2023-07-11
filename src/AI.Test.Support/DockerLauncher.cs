using DotNet.Testcontainers.Configurations;

namespace AI.Test.Support;

public class DockerLauncher
{
    public DockerLauncher(/* get options and launch the docker containers*/)
    {
    }

    //docker desktop settings /temp/qdrant_storage     /qdrant/storage  63333 6334
    //ensure singleton 
    private static bool isLaunched = false;

    public void Launch(/* get options and lacunh the docker containers*/)
    {
        if (isLaunched) return;
        //https://www.lambdatest.com/automation-testing-advisor/csharp/methods/DotNet.Testcontainers.Builders.TestcontainersBuilderTDockerContainer.WithVolumeMount
        var sourcePath = Path.GetFullPath(Path.Combine("/temp", "qdrant_storage"));
        if (!Directory.Exists(sourcePath))
        {
            Directory.CreateDirectory(sourcePath);
        }

        try
        {
            TestContainerFactory.Build("qdrant/qdrant:latest", 6333, 6333, 6333,
                b => b.WithBindMount(sourcePath, "/qdrant/storage", AccessMode.ReadWrite)
            ).Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        isLaunched = true;
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
