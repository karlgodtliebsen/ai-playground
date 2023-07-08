using System.Security.Claims;

namespace AI.CaaP.WebAPI.Controllers;

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
            var s = identity.FindFirst("sub")!.Value;
            if (Guid.TryParse(s, out var guid))
            {
                this.UserId = guid;
            }
            else
            {
                this.UserId = Guid.NewGuid();
            }
        }
    }

    /// <inheritdoc />
    public Guid UserId { get; set; } = Guid.NewGuid();
}
