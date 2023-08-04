using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace LLamaSharpApp.WebAPI.Configuration.LibraryConfiguration;

/// <summary>
/// Azure Ad Options
/// </summary>
public class AzureAdOptions
{
    /// <summary>
    /// DefaultSectionName
    /// </summary>
    public const string SectionName = Constants.AzureAd;


    /// <summary>
    /// 
    /// </summary>
    public string JwtBearerScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;

    /// <summary>
    /// 
    /// </summary>
    public bool SubscribeToJwtBearerMiddlewareDiagnosticsEvents { get; set; } = false;
}
