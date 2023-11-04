namespace Kernel.Memory.NewsFeed.Domain.Configuration;

public class OpenSearchConfiguration
{
    public const string SectionName = "OpenSearch";

    public string UserName { get; set; } = "admin";
    public string Password { get; set; } = "admin";

    public string EndPoint { get; set; } = "http://localhost:9200/";
}
