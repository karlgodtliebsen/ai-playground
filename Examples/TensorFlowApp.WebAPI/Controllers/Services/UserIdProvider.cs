using System.Security.Claims;

namespace TensorFlowApp.WebAPI.Controllers.Services;

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
                this.UserId = guid.ToString();
            }
            else
            {
                this.UserId = Guid.NewGuid().ToString();
            }
        }
    }

    /// <inheritdoc />
    public string UserId { get; set; } = Guid.NewGuid().ToString();
}
