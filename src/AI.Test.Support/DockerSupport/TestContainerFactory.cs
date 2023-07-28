﻿using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace AI.Test.Support.DockerSupport;

public static class TestContainerFactory
{
    /// <summary>
    /// Starts a docker instance of the specified image
    /// <a href="https://dotnet.testcontainers.org" />
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="hostPort">Bind specified port of the container to this port on the host</param>
    /// <param name="containerPort">Bind the specified port of the container to the hostport</param>
    /// <param name="waitForPort">The port to check. Waits for the port to respond</param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IContainer Build(string imageName, int hostPort, int containerPort, ushort waitForPort,
                                Func<ContainerBuilder, ContainerBuilder>? builder = null)
    {
        var containerBuilder = new ContainerBuilder()
            // Set the image for the container to "testcontainers/helloworld:1.1.0".
            .WithImage(imageName)
            // Bind port 'hostPort' of the container to a 'containerPort' port on the host.
            .WithPortBinding(hostPort, containerPort)
            // Wait until the HTTP endpoint of the container is available.
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(waitForPort)));

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
}