using System.Text.Json.Serialization;

using AI.Library.Utils;
using AI.Test.Support;

using DotNet.Testcontainers.Configurations;

using Microsoft.Extensions.Options;

using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace Embeddings.Qdrant.Tests.Fixtures;


/*
 https://github.com/qdrant/qdrant/blob/master/QUICK_START.md
docker run -p 6333:6333 qdrant/qdrant
docker run -p 6333:6333 -v ./qdrant_storage:/qdrant/storage qdrant/qdrant:latest
docker run -p 6333:6333 \
-v $(pwd)/path/to/data:/qdrant/storage \
-v $(pwd)/path/to/custom_config.yaml:/qdrant/config/production.yaml \
qdrant/qdrant
}
//docker desktop settings /temp/qdrant_storage     /qdrant/storage  63333 6334
//ensure singleton 
*/

public class TestContainerDockerLauncher
{
    private readonly ILogger logger;
    private readonly DockerLaunchOptions options;
    private static bool isLaunched = false;
    readonly IList<IContainer> containers = new List<IContainer>();

    public TestContainerDockerLauncher(IOptions<DockerLaunchOptions> options, ILogger logger)
    {
        this.logger = logger;
        this.options = options.Value;
    }

    public void Start(Action<DockerLaunchOptions>? setOptions = null)
    {
        if (isLaunched) return;
        //https://www.lambdatest.com/automation-testing-advisor/csharp/methods/DotNet.Testcontainers.Builders.TestcontainersBuilderTDockerContainer.WithVolumeMount

        if (setOptions is not null)
        {
            setOptions(options);
        }

        foreach (var option in options.DockerSettings)
        {
            try
            {

                if (option.HostPath is not null)
                {
                    var sourcePath = Path.GetFullPath(option.HostPath);
                    var mappedTo = option.ContainerPath;
                    if (!Directory.Exists(sourcePath))
                    {
                        Directory.CreateDirectory(sourcePath);
                    }
                    var container = TestContainerFactory
                       .Build(option.ImageName, option.HostPort, option.ContainerPort, option.WaitForPort,
                           b => b.WithBindMount(sourcePath, mappedTo, AccessMode.ReadWrite)
                       );

                    if (container is not null)
                    {
                        containers.Add(container);
                    }
                }
                else
                {
                    var container = TestContainerFactory.Build(option.ImageName, option.HostPort, option.ContainerPort, option.WaitForPort);
                    if (container is not null)
                    {
                        containers.Add(container);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error starting Docker Container for\n{options}", options.ToJson());
            }
        }
        isLaunched = true;
    }

    public void Stop()
    {
        foreach (var container in containers)
        {
            try
            {
                container.StopAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Stopping Docker Container for\n{options}", options.ToJson());
            }
        }
    }
}

public class DockerLaunchOptions
{
    public const string SectionName = "DockerLaunch";

    [JsonPropertyName("DockerSettings")]
    public DockerLaunchOption[] DockerSettings { get; set; } = { };
}

// "/temp/qdrant_storage", "/qdrant/storage"

public class DockerLaunchOption
{
    public string ImageName { get; set; } = default!;
    public int HostPort { get; set; } = 6333;
    public int ContainerPort { get; set; } = 6333;
    public ushort WaitForPort { get; set; } = 6333;

    public string? HostPath { get; set; } = default!;
    public string? ContainerPath { get; set; } = default!;
}



//var sourcePath = Path.GetFullPath(Path.Combine("/temp", "qdrant_storage"));
//if (!Directory.Exists(sourcePath))
//{
//    Directory.CreateDirectory(sourcePath);
//}
//try
//{
//    TestContainerFactory
//        .Build("qdrant/qdrant:latest", 6333, 6333, 6333, b => b
//        .WithBindMount(sourcePath, "/qdrant/storage", AccessMode.ReadWrite)
//    ).Wait();
//}
//catch (Exception e)
//{
//    Console.WriteLine(e);
//}
