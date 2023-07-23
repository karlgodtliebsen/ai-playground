using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.Extensions.Options;

namespace AI.Test.Support.DockerSupport;

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

        logger.Information("Starting Docker Containers with Options:\n{@options}", options);
        foreach (var option in options.DockerSettings)
        {
            try
            {
                logger.Information("Stating Docker Container Image:\n {option}", option.ImageName);
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
                logger.Error(ex, "Error starting Docker Container for\n{@options}", options);
            }
        }
        isLaunched = true;
    }

    public void Stop()
    {
        logger.Information("Stopping Docker Containers...");
        foreach (var container in containers)
        {
            try
            {
                container.StopAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Stopping Docker Container for\n{@options}", options);
            }
        }
    }
}


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

//public class TestContainerDockerLauncher
//{
//    private readonly DockerLaunchOptions options;
//    private static bool isLaunched = false;
//    readonly IList<IContainer> containers = new List<IContainer>();

//    public TestContainerDockerLauncher(IOptions<DockerLaunchOptions> options)
//    {
//        this.options = options.Value;
//    }
//    public void Start(IMessageSink diagnosticMessageSink, Action<DockerLaunchOptions>? setOptions = null)
//    {
//        var message = new DiagnosticMessage("Starting Docker Containers...");
//        diagnosticMessageSink.OnMessage(message);

//        if (isLaunched) return;
//        //https://www.lambdatest.com/automation-testing-advisor/csharp/methods/DotNet.Testcontainers.Builders.TestcontainersBuilderTDockerContainer.WithVolumeMount

//        if (setOptions is not null)
//        {
//            setOptions(options);
//        }

//        foreach (var option in options.DockerSettings)
//        {
//            try
//            {
//                message = new DiagnosticMessage("Docker Container {0 }", option);
//                diagnosticMessageSink.OnMessage(message);
//                if (option.HostPath is not null)
//                {
//                    var sourcePath = Path.GetFullPath(option.HostPath);
//                    var mappedTo = option.ContainerPath;
//                    if (!Directory.Exists(sourcePath))
//                    {
//                        Directory.CreateDirectory(sourcePath);
//                    }
//                    var container = TestContainerFactory
//                       .Build(option.ImageName, option.HostPort, option.ContainerPort, option.WaitForPort,
//                           b => b.WithBindMount(sourcePath, mappedTo, AccessMode.ReadWrite)
//                       );
//                    containers.Add(container);
//                }
//                else
//                {
//                    var container = TestContainerFactory.Build(option.ImageName, option.HostPort, option.ContainerPort, option.WaitForPort);
//                    containers.Add(container);
//                }
//            }
//            catch (Exception ex)
//            {
//                message = new DiagnosticMessage("Error {0} \n starting Docker Container for\n{1}", ex.ToString(), options.ToJson());
//                diagnosticMessageSink.OnMessage(message);
//            }
//        }
//        isLaunched = true;
//    }

//    public void Stop(IMessageSink diagnosticMessageSink)
//    {
//        foreach (var container in containers)
//        {
//            try
//            {
//                container.StopAsync().Wait();
//            }
//            catch (Exception ex)
//            {
//                var message = new DiagnosticMessage("Error {0} \nStopping Docker Container for\n{1}", ex.ToString(), options.ToJson());
//                diagnosticMessageSink.OnMessage(message);
//            }
//        }
//    }
//}
