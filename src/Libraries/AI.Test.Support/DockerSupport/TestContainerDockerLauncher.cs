using Microsoft.Extensions.Options;

namespace AI.Test.Support.DockerSupport;

public class TestContainerDockerLauncher
{
    private readonly ILogger logger;
    private DockerLaunchOptions options;
    private static bool isLaunched = false;
    readonly IList<IContainer> containers = new List<IContainer>();

    public TestContainerDockerLauncher(IOptions<DockerLaunchOptions> options, ILogger logger)
    {
        this.logger = logger;
        this.options = options.Value;
    }

    public void SetOptions(DockerLaunchOptions setOptions)
    {
        this.options = setOptions;
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
                logger.Information("Stating Docker Container Image:\n {image} {name}", option.ImageName, option.ContainerName);
                if (option.HostPath is not null)
                {
                    var sourcePath = Path.GetFullPath(option.HostPath);
                    var mappedTo = option.ContainerPath;
                    if (!Directory.Exists(sourcePath))
                    {
                        Directory.CreateDirectory(sourcePath);
                    }
                    var container = TestContainerFactory.Build(option, b => b.WithBindMount(sourcePath, mappedTo, AccessMode.ReadWrite));
                    containers.Add(container);
                }
                else
                {
                    var container = TestContainerFactory.Build(option);
                    containers.Add(container);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error starting Docker Container for\n {image} {name}", option.ImageName, option.ContainerName);
            }
        }
        isLaunched = true;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (isLaunched) return;
        //https://www.lambdatest.com/automation-testing-advisor/csharp/methods/DotNet.Testcontainers.Builders.TestcontainersBuilderTDockerContainer.WithVolumeMount

        logger.Information("Starting Docker Containers with Options:\n{@options}", options);
        foreach (var option in options.DockerSettings)
        {
            try
            {
                logger.Information("Stating Docker Container Image:\n {image} {name}", option.ImageName, option.ContainerName);
                if (option.HostPath is not null)
                {
                    var sourcePath = Path.GetFullPath(option.HostPath);
                    var mappedTo = option.ContainerPath;
                    if (!Directory.Exists(sourcePath))
                    {
                        Directory.CreateDirectory(sourcePath);
                    }
                    var container = await TestContainerFactory.BuildAsync(option, cancellationToken, b => b.WithBindMount(sourcePath, mappedTo, AccessMode.ReadWrite));
                    containers.Add(container);
                }
                else
                {
                    var container = await TestContainerFactory.BuildAsync(option, cancellationToken);
                    containers.Add(container);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error starting Docker Container for\n {image} {name}", option.ImageName, option.ContainerName);
            }
        }
        isLaunched = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!isLaunched) return;
        logger.Information("Stopping Docker Containers...");
        foreach (var container in containers)
        {
            try
            {
                await container.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Stopping Docker Container for\n{@options}", options);
            }
        }
        isLaunched = false;
    }

    public void Stop()
    {
        if (!isLaunched) return;
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
        isLaunched = false;
    }
}
