namespace LLamaSharpApp.WebAPI.Controllers.Services;

/// <summary>
/// User id provider
/// </summary>
public class UserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    public UserIdProvider()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userUserId"></param>
    public UserIdProvider(string userUserId)
    {
        this.UserId = userUserId;
    }

    /// <inheritdoc />
    public string UserId { get; set; } = "42";// so this must be replaced with an authentication setup
}
