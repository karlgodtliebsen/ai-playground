using Destructurama.Attributed;

namespace AI.Library.Configuration;

public class AzureKeyVaultOptions
{
    public const string ConfigSectionName = "AzureKeyVaultOptions";

    /// <summary>
    /// Configuration Instance SectionName
    /// </summary>
    public string SectionName { get; set; } = ConfigSectionName;

    /// <summary>
    /// Name of KeyVault 
    /// </summary>
    public string? KeyVaultName { get; set; } = default!;

    /// <summary>
    /// Thumbprint of the certificate
    /// </summary>
    [LogMasked]
    public string? CertificateThumbprint { get; set; } = default!;

    /// <summary>
    /// Azure Active Directory Id (Tenant id)
    /// </summary>
    [LogMasked]
    public string? DirectoryId { get; set; } = default!;

    /// <summary>
    /// Client id of the Application
    /// </summary>
    [LogMasked]
    public string? ApplicationId { get; set; } = default!;
}
