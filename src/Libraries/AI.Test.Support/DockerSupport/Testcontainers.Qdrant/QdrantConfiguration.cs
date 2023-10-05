﻿namespace AI.Test.Support.DockerSupport.Testcontainers.Qdrant;

/// <inheritdoc cref="ContainerConfiguration" />
//[PublicAPI]
public sealed class QdrantConfiguration : ContainerConfiguration
{

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="hostPath"></param>
    /// <param name="containerPath"></param>
    /// <param name="accessMode"></param>

    public QdrantConfiguration(
        string? containerName = null,
        string? hostPath = null,
        string? containerPath = null,
        AccessMode? accessMode = null
    )
    {
        ContainerName = containerName;
        HostPath = hostPath;
        ContainerPath = containerPath;
        if (accessMode is not null)
        {
            AccessMode = accessMode.Value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public QdrantConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public QdrantConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public QdrantConfiguration(QdrantConfiguration resourceConfiguration)
        : this(new QdrantConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public QdrantConfiguration(QdrantConfiguration oldValue, QdrantConfiguration newValue)
        : base(oldValue, newValue)
    {
        ContainerName = BuildConfiguration.Combine(oldValue.ContainerName, newValue.ContainerName);
        HostPath = BuildConfiguration.Combine(oldValue.HostPath, newValue.HostPath);
        ContainerPath = BuildConfiguration.Combine(oldValue.ContainerPath, newValue.ContainerPath);
        AccessMode = BuildConfiguration.Combine(oldValue.AccessMode, newValue.AccessMode);
    }

    /// <summary>
    /// Gets the docker container name.
    /// </summary>
    public string? ContainerName { get; }

    /// <summary>
    /// Gets the HostPath for mounting volumes.
    /// </summary>
    public string? HostPath { get; }

    /// <summary>
    /// Gets the ContainerPath  for mounting volumes.
    /// </summary>
    public string? ContainerPath { get; }

    //public string AccessMode { get; } = "rw";
    public AccessMode AccessMode { get; } = AccessMode.ReadWrite;

    //var mode = accessMode == "ro" ? AccessMode.ReadOnly : AccessMode.ReadWrite;

}
