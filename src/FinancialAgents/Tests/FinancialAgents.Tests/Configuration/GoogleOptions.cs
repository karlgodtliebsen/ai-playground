namespace FinancialAgents.Tests.Configuration;

public class GoogleOptions
{    /// <summary>
     /// Configuration Section Name
     /// </summary>
    public const string SectionName = "Google";

    public string SearchEngineId { get; set; }


    public string ApiKey { get; set; } = "n/a";
}
