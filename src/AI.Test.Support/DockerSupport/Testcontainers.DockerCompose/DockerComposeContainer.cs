
using System.Collections.Generic;

namespace AI.Test.Support.DockerSupport.Testcontainers.DockerCompose;

/// <inheritdoc cref="DockerContainer" />
//[PublicAPI]
public sealed class DockerComposeContainer : DockerContainer
{
    private readonly DockerComposeConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DockerComposeContainer(DockerComposeConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Qdrant connection url.
    /// </summary>
    /// <returns>The Qdrant connection url.</returns>
    public string GetConnectionUrl()
    {
        return $"http://{Hostname}:{GetMappedPublicPort(DockerComposeBuilder.QdrantPort)}";
    }
}

//public static DockerComposeContainer environment =
//    new DockerComposeContainer(new File("src/test/resources/compose-test.yml"))
//        .withExposedService("redis_1", REDIS_PORT)
//        .withExposedService("elasticsearch_1", ELASTICSEARCH_PORT);


public class DockerComposeFiles
{

    //private final List<ParsedDockerComposeFile> parsedComposeFiles;

    //public DockerComposeFiles(List<File> composeFiles)
    //{
    //    this.parsedComposeFiles = composeFiles.stream().map(ParsedDockerComposeFile::new).collect(Collectors.toList());
    //}

    //public Set<String> getDependencyImages()
    //{
    //    Map<String, Set<String>> mergedServiceNameToImageNames = mergeServiceDependencyImageNames();

    //    return getImageNames(mergedServiceNameToImageNames);
    //}

    //private Map<String, Set<String>> mergeServiceDependencyImageNames()
    //{
    //    Map<String, Set<String>> mergedServiceNameToImageNames = new HashMap<>();
    //    for (ParsedDockerComposeFile parsedComposeFile : parsedComposeFiles)
    //    {
    //        mergedServiceNameToImageNames.putAll(parsedComposeFile.getServiceNameToImageNames());
    //    }
    //    return mergedServiceNameToImageNames;
    //}
    //private Set<String> getImageNames(Map<String, Set<String>> serviceToImageNames)
    //{
    //    return serviceToImageNames
    //        .values()
    //        .stream()
    //        .flatMap(Collection::stream)
    //        // Pass through DockerImageName to convert image names to canonical form (e.g. making implicit latest tag explicit)
    //        .map(DockerImageName::parse)
    //        .map(DockerImageName::asCanonicalNameString)
    //        .collect(Collectors.toSet());
    //}
}

interface DockerCompose
{
    //String ENV_PROJECT_NAME = "COMPOSE_PROJECT_NAME";
    //String ENV_COMPOSE_FILE = "COMPOSE_FILE";

    //DockerCompose withCommand(String cmd);

    //DockerCompose withEnv(Map<String, String> env);

    //void invoke();
}
//class ContainerisedDockerCompose extends GenericContainer<ContainerisedDockerCompose> implements DockerCompose {

//    public static final char UNIX_PATH_SEPARATOR = ':';

//public ContainerisedDockerCompose(DockerImageName dockerImageName, List<File> composeFiles, String identifier)
//{
//    super(dockerImageName);
//    addEnv(ENV_PROJECT_NAME, identifier);

//    // Map the docker compose file into the container
//    final File dockerComposeBaseFile = composeFiles.get(0);
//    final String pwd = dockerComposeBaseFile.getAbsoluteFile().getParentFile().getAbsolutePath();
//    final String containerPwd = convertToUnixFilesystemPath(pwd);

//    final List<String> absoluteDockerComposeFiles = composeFiles
//        .stream()
//        .map(File::getAbsolutePath)
//        .map(MountableFile::forHostPath)
//        .map(MountableFile::getFilesystemPath)
//        .map(this::convertToUnixFilesystemPath)
//        .collect(Collectors.toList());
//    final String composeFileEnvVariableValue = Joiner.on(UNIX_PATH_SEPARATOR).join(absoluteDockerComposeFiles); // we always need the UNIX path separator
//    logger().debug("Set env COMPOSE_FILE={}", composeFileEnvVariableValue);
//    addEnv(ENV_COMPOSE_FILE, composeFileEnvVariableValue);
//    withCopyFileToContainer(MountableFile.forHostPath(pwd), containerPwd);

//    // Ensure that compose can access docker. Since the container is assumed to be running on the same machine
//    //  as the docker daemon, just mapping the docker control socket is OK.
//    // As there seems to be a problem with mapping to the /var/run directory in certain environments (e.g. CircleCI)
//    //  we map the socket file outside of /var/run, as just /docker.sock
//    addFileSystemBind(
//        DockerClientFactory.instance().getRemoteDockerUnixSocketPath(),
//        "/docker.sock",
//        BindMode.READ_WRITE
//    );
//    addEnv("DOCKER_HOST", "unix:///docker.sock");
//    setStartupCheckStrategy(new IndefiniteWaitOneShotStartupCheckStrategy());
//    setWorkingDirectory(containerPwd);
//}

//@Override
//    public void invoke()
//{
//    super.start();

//    this.followOutput(new Slf4jLogConsumer(logger()));

//    // wait for the compose container to stop, which should only happen after it has spawned all the service containers
//    logger()
//        .info("Docker Compose container is running for command: {}", Joiner.on(" ").join(this.getCommandParts()));
//    while (this.isRunning())
//    {
//        logger().trace("Compose container is still running");
//        Uninterruptibles.sleepUninterruptibly(100, TimeUnit.MILLISECONDS);
//    }
//    logger().info("Docker Compose has finished running");

//    AuditLogger.doComposeLog(this.getCommandParts(), this.getEnv());

//    final Integer exitCode = getDockerClient()
//        .inspectContainerCmd(getContainerId())
//        .exec()
//        .getState()
//        .getExitCode();

//    if (exitCode == null || exitCode != 0)
//    {
//        throw new ContainerLaunchException(
//            "Containerised Docker Compose exited abnormally with code " +
//            exitCode +
//            " whilst running command: " +
//            StringUtils.join(this.getCommandParts(), ' ')
//        );
//    }
//}

//private String convertToUnixFilesystemPath(String path)
//{
//    return SystemUtils.IS_OS_WINDOWS ? PathUtils.createMinGWPath(path).substring(1) : path;
//}
//}
