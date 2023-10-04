using Testcontainers.Elasticsearch;

namespace AI.Library.Configuration;

/// <summary>
/// Options for OpenSearch
/// </summary>
public sealed class OpenSearchOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "OpenSearch";

    /// <summary>
    /// Use this Connection string
    /// </summary>
    public string? ConnectionString { get; set; } = default!;

    public string? Username { get; set; } = default!;
    public string? Password { get; set; } = default!;

    public ElasticsearchConfiguration ElasticsearchConfiguration
    {
        get
        {
            return new ElasticsearchConfiguration(Username, Password);
        }
    }

}
