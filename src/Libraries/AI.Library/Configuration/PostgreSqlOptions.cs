namespace AI.Library.Configuration;

/// <summary>
/// Options for PostgreSql
/// </summary>
public sealed class PostgreSqlOptions
{
    /// <summary>
    /// Configuration SectionName
    /// </summary>
    public const string SectionName = "PostgreSql";

    /// <summary>
    /// Use this Connection string
    /// </summary>
    public string? ConnectionString { get; set; } = default!;

}
