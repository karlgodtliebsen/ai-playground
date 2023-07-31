
namespace AI.Library.Utils;

public static class HostingEnvironments
{
    /// <summary>
    /// Supports a scenario where the web application is running in  a docker container, using nginx as a reverse proxy, that terminates the SSL connection.
    /// </summary>
    public const string UsingReverseProxy = "UsingReverseProxy";
}
