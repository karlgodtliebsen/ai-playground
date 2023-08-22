using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace AI.Test.Support.DockerSupport;

public static class TestContainerFactory
{
    /// <summary>
    /// Starts a docker instance of the specified image
    /// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
    /// <a href="https://dotnet.testcontainers.org">Source</a>
    /// </summary>
    /// <param name="option"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IContainer Build(DockerLaunchOption option,

        //string imageName, int hostPort, int containerPort, ushort waitForPort, IDictionary<string, string>? environment = null,

        Func<ContainerBuilder, ContainerBuilder>? builder = null)
    {
        var containerBuilder = new ContainerBuilder()
            // Set the image for the container to, sample: "testcontainers/helloworld:1.1.0".
            .WithImage(option.ImageName)
            .WithName(option.ContainerName)
            // Bind port 'hostPort' of the container to a 'containerPort' port on the host.
            .WithPortBinding(option.HostPort, option.ContainerPort)
            // Wait until the HTTP endpoint of the container is available.
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(option.WaitForPort)));

        if (option.EnvironmentVars is not null)
        {
            foreach (var kvp in option.EnvironmentVars)
            {
                containerBuilder = containerBuilder.WithEnvironment(kvp.Key, kvp.Value);
            }
        }

        if (builder is not null)
        {
            containerBuilder = builder(containerBuilder);
        }

        // Build the container configuration.
        var container = containerBuilder.Build();
        if (container.State == TestcontainersStates.Running)
        {
            return container;
        }
        //To allow call from non-async methods
        container.StartAsync().Wait();
        return container;
    }

    /// <summary>
    /// Starts a docker instance of the specified image
    /// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
    /// <a href="https://dotnet.testcontainers.org">Source</a>
    /// </summary>
    /// <param name="option"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static async Task<IContainer> BuildAsync(DockerLaunchOption option, CancellationToken cancellationToken, Func<ContainerBuilder, ContainerBuilder>? builder = null)
    {
        var containerBuilder = new ContainerBuilder()
            // Set the image for the container to, sample: "testcontainers/helloworld:1.1.0".
            .WithImage(option.ImageName)
            .WithName(option.ContainerName)
            // Bind port 'hostPort' of the container to a 'containerPort' port on the host.
            .WithPortBinding(option.HostPort, option.ContainerPort)
            // Wait until the HTTP endpoint of the container is available.
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(option.WaitForPort)));

        if (option.EnvironmentVars is not null)
        {
            foreach (var kvp in option.EnvironmentVars)
            {
                containerBuilder = containerBuilder.WithEnvironment(kvp.Key, kvp.Value);
            }
        }

        if (builder is not null)
        {
            containerBuilder = builder(containerBuilder);
        }

        // Build the container configuration.
        var container = containerBuilder.Build();
        if (container.State == TestcontainersStates.Running)
        {
            return container;
        }
        //To allow call from non-async methods
        await container.StartAsync(ct: cancellationToken);
        return container;
    }
}
