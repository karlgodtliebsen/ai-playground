namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public class OpenSearchConfiguration
{
    public const string SectionName = "OpenSearch";

    public string UserName { get; set; }
    public string Password { get; set; }

    public string EndPoint { get; set; }

    public string[] Indices { get; set; } = Array.Empty<string>();

}
