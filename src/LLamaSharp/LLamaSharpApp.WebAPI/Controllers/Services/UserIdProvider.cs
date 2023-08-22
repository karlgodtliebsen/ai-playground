using System.Security.Claims;

namespace LLamaSharpApp.WebAPI.Controllers.Services;

/// <summary>
/// Placeholder for User id provider, Can be used together with authentication/identity or it can be replaced
/// Reference: <a href="https://damienbod.com/" />
/// /// </summary>
public class UserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    public UserIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor!.HttpContext!.User.Identity is ClaimsIdentity identity)
        {
            var sId = identity.FindFirst("sub");
            if (sId is null)
            {
                this.UserId = Guid.Empty.ToString("N");
                return;
            }
            this.UserId = Guid.TryParse(sId!.Value, out var guid) ? guid.ToString("N") : Guid.NewGuid().ToString("N");
        }
    }

    /// <inheritdoc />
    public string UserId { get; set; } = default!;
}
