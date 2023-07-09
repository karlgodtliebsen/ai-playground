using Destructurama.Attributed;

namespace AI.CaaP.Repository.Configuration;

public class DatabaseConnectionOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public static string ConfigSectionName { get; set; } = "DatabaseConnection";

    /// <summary>
    /// Configuration Instance SectionName
    /// </summary>
    public string SectionName { get; set; } = ConfigSectionName;


    [LogMasked]
    public string ConnectionString { get; set; } = null!;

    public string MigrationAssembly { get; set; } = null!;


    public string UseProvider { get; set; } = "mssql";
}