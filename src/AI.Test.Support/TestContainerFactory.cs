﻿using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace AI.Test.Support;

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
    /// <returns></returns>
    public static async Task<IContainer> Build(string imageName, int hostPort, int containerPort, ushort waitForPort)
    {
        var container = new ContainerBuilder()
            // Set the image for the container to "testcontainers/helloworld:1.1.0".
            .WithImage(imageName)
            // Bind port 'hostPort' of the container to a 'containerPort' port on the host.
            .WithPortBinding(hostPort, containerPort)
            // Wait until the HTTP endpoint of the container is available.
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPort(waitForPort)))
            // Build the container configuration.
            .Build();

        if (container.State == TestcontainersStates.Running)
        {
            return container;
        }

        // Start the container.
        await container.StartAsync().ConfigureAwait(false);
        return container;
    }
}
