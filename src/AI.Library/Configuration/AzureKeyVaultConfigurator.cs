using System.Security.Cryptography.X509Certificates;

using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Serilog;

namespace AI.Library.Configuration;

/// <summary>
/// Configurator for using Azure KeyVault with Local Machine Certificate
/// </summary>
public static class AzureKeyVaultConfigurator
{
    /// <summary>
    ///  Uses Local Machine Certificate matching thumbprint to add Azure KeyVault with the possibility to override the appSettings section name and other options
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options">Option used to locate the settings in appSettings</param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAzureKeyVault(this WebApplicationBuilder builder, Action<AzureKeyVaultOptions>? options = null, string? sectionName = null)
    {
        sectionName ??= AzureKeyVaultOptions.SectionName;
        var useOptions = new AzureKeyVaultOptions();
        options?.Invoke(useOptions);
        var keyVaultOptions = builder.Configuration.GetSection(sectionName).Get<AzureKeyVaultOptions>()!;
        ArgumentNullException.ThrowIfNull(keyVaultOptions);
        if (useOptions.ApplicationId is not null)
        {
            keyVaultOptions.ApplicationId = useOptions.ApplicationId;
        }
        if (useOptions.CertificateThumbprint is not null)
        {
            keyVaultOptions.CertificateThumbprint = useOptions.CertificateThumbprint;
        }
        if (useOptions.DirectoryId is not null)
        {
            keyVaultOptions.DirectoryId = useOptions.DirectoryId;
        }
        if (useOptions.KeyVaultName is not null)
        {
            keyVaultOptions.KeyVaultName = useOptions.KeyVaultName;
        }
        ArgumentNullException.ThrowIfNull(keyVaultOptions);
        ArgumentNullException.ThrowIfNull(keyVaultOptions.ApplicationId);
        ArgumentNullException.ThrowIfNull(keyVaultOptions.CertificateThumbprint);
        ArgumentNullException.ThrowIfNull(keyVaultOptions.DirectoryId);
        ArgumentNullException.ThrowIfNull(keyVaultOptions.KeyVaultName);
        AddAzureKeyVaultSecretsUsingCertificate(builder.Configuration, keyVaultOptions);
        return builder;
    }


    /// <summary>
    ///  Uses Local Machine Certificate matching thumbprint to get access to Secrets in Azure KeyVault
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="options"></param>
    public static void AddAzureKeyVaultSecretsUsingCertificate(IConfigurationBuilder configurationBuilder, AzureKeyVaultOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.ApplicationId);
        ArgumentNullException.ThrowIfNull(options.CertificateThumbprint);
        ArgumentNullException.ThrowIfNull(options.DirectoryId);
        ArgumentNullException.ThrowIfNull(options.KeyVaultName);

        using var x509Store = new X509Store(StoreLocation.LocalMachine);
        x509Store.Open(OpenFlags.ReadOnly);

        var x509Certificate = x509Store.Certificates
            .Find(X509FindType.FindByThumbprint, options.CertificateThumbprint!, validOnly: false)
            .SingleOrDefault();
        if (x509Certificate is null)
        {
            Log.Logger.Error("Could not find specified Certificate: {thumbPrint}", options.CertificateThumbprint);
            return;
        }

        configurationBuilder.AddAzureKeyVault(
            new Uri($"https://{options.KeyVaultName}.vault.azure.net/"),
            new ClientCertificateCredential(options.DirectoryId, options.ApplicationId, x509Certificate),
            new AzureKeyVaultConfigurationOptions());

        return;
    }


    /// <summary>
    /// Uses Local Machine Certificate matching thumbprint to get access to Secrets in Azure KeyVault
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAzureKeyVaultSecretsUsingCertificate(this WebApplicationBuilder builder, Action<AzureKeyVaultOptions> options)
    {
        var keyVaultOptions = new AzureKeyVaultOptions();
        options.Invoke(keyVaultOptions);
        AddAzureKeyVaultSecretsUsingCertificate(builder.Configuration, keyVaultOptions);
        return builder;
    }

    /// <summary>
    ///  Uses Local Machine Certificate matching thumbprint to add Azure KeyVault Secrets Using Default Section 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddAzureKeyVaultSecretsUsingCertificate(this WebApplicationBuilder builder, Action<AzureKeyVaultOptions>? options = null, string? sectionName = null)
    {
        sectionName ??= AzureKeyVaultOptions.SectionName;
        var keyVaultOptions = builder.Configuration.GetSection(sectionName).Get<AzureKeyVaultOptions>()!;
        options?.Invoke(keyVaultOptions);
        AddAzureKeyVaultSecretsUsingCertificate(builder.Configuration, keyVaultOptions);
        return builder;
    }

    /// <summary>
    /// Add Support for Azure KeyVault Secrets Using ManagedIdentity
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddSecretsUsingManagedIdentity(this WebApplicationBuilder builder, AzureKeyVaultOptions options)
    {
        AddSecretsUsingManagedIdentity(builder.Configuration, options);
        return builder;
    }

    private static Uri CreateUri(string keyVaultName)
    {
        return new Uri($"https://{keyVaultName}.vault.azure.net/");
    }

    /// <summary>
    /// Add Support for Azure KeyVault Secrets Using ManagedIdentity
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static void AddSecretsUsingManagedIdentity(IConfigurationBuilder configurationBuilder, AzureKeyVaultOptions options)
    {
        var secretOptions = new SecretClientOptions()
        {
            Retry =
            {
                Delay = TimeSpan.FromSeconds(2), MaxDelay = TimeSpan.FromSeconds(10), MaxRetries = 3, Mode = RetryMode.Exponential
            }
        };

        var secretClient = new SecretClient(CreateUri(options.KeyVaultName!), new DefaultAzureCredential(), secretOptions);
        configurationBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }


    /// <summary>
    /// Get the specified Secret 
    /// </summary>
    /// <param name="azureKeyVaultOptions"></param>
    /// <param name="secretKey"></param>
    /// <returns></returns>
    public static string GetSpecifiedSecret(AzureKeyVaultOptions azureKeyVaultOptions, string secretKey)
    {
        var secretOptions = new SecretClientOptions()
        {
            Retry =
            {
                Delay = TimeSpan.FromSeconds(2), MaxDelay = TimeSpan.FromSeconds(10), MaxRetries = 3, Mode = RetryMode.Exponential
            }
        };
        var secretClient = new SecretClient(CreateUri(azureKeyVaultOptions.KeyVaultName!), new DefaultAzureCredential(), secretOptions);
        KeyVaultSecret secret = secretClient.GetSecret(secretKey);
        string secretValue = secret.Value;
        return secretValue;
    }
}
