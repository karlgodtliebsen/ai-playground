using AI.Test.Support.DockerSupport.Testcontainers.Qdrant;

namespace AI.Test.Support.DockerSupport.Testcontainers.DockerCompose;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
//[PublicAPI]
public sealed class DockerComposeBuilder : ContainerBuilder<DockerComposeBuilder, DockerComposeContainer, DockerComposeConfiguration>
{
    public const string QdrantImage = "qdrant/qdrant:latest";

    public const ushort QdrantPort = 6333;

    public const string QdrantContainerName = "qdrant";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeBuilder" /> class.
    /// </summary>
    public DockerComposeBuilder() : this(new DockerComposeConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DockerComposeBuilder(DockerComposeConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DockerComposeConfiguration DockerResourceConfiguration { get; }


    /// <summary>
    /// Sets the Volume Mapping.
    /// <a href="https://docs.docker.com/storage/volumes/">Docker Volume</a>
    /// <a href="https://docs.docker.com/storage/bind-mounts/">Docker Bind Mount</a>
    /// <a href="https://hub.docker.com/r/qdrant/qdrant/">Qdrant</a>
    /// </summary>
    /// <param name="hostPath">The mapped Host Path .</param>
    /// <param name="containerPath">The mapped Container Path.</param>
    /// <param name="accessMode">The AccessMode.</param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithVolume(string hostPath, string containerPath, AccessMode accessMode)
    {
        return
            Merge(DockerResourceConfiguration,
                new DockerComposeConfiguration(hostPath: hostPath, containerPath: containerPath, accessMode: accessMode))
            ;
    }

    public DockerComposeBuilder WithContainerName(string containerName)
    {
        return
            Merge(DockerResourceConfiguration,
                new DockerComposeConfiguration(containerName: containerName))
            ;
    }

    /// <inheritdoc />
    public override DockerComposeContainer Build()
    {
        Validate();
        return new DockerComposeContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Init()
    {
        var containerBuilder = base.Init()
            .WithImage(QdrantImage)
            .WithPortBinding(QdrantPort, true)
            .BuildWithContainerName(DockerResourceConfiguration.ContainerName)
            .BuildWithVolume(DockerResourceConfiguration.HostPath, DockerResourceConfiguration.ContainerPath, DockerResourceConfiguration.AccessMode);
        containerBuilder = containerBuilder.WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
        return containerBuilder;
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Merge(DockerComposeConfiguration oldValue, DockerComposeConfiguration newValue)
    {
        return new DockerComposeBuilder(new DockerComposeConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, stderr) = await container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            return stdout.Contains("Access web UI at http:");
        }
    }
}

internal static class DockerComposeBuilderExtensions
{
    public static DockerComposeBuilder BuildWithContainerName(this DockerComposeBuilder builder, string? containerName)
    {
        if (containerName is not null)
        {
            builder = builder.WithName(containerName);
        }
        return builder;
    }

    public static DockerComposeBuilder BuildWithVolume(this DockerComposeBuilder builder, string? hostPath, string? containerPath, AccessMode accessMode)
    {
        if (hostPath is not null)
        {
            builder = builder.WithBindMount(hostPath, containerPath, accessMode);
        }
        return builder;
    }
}
