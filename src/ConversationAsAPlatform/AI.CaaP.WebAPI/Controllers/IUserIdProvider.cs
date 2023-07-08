namespace AI.CaaP.WebAPI.Controllers;

/// <summary>
/// Add this interface to the controller that needs to get the user id from the request.
/// </summary>
public interface IUserIdProvider
{
    /// <summary>
    /// The uers id that can be used for session / state persistence
    /// </summary>
    Guid UserId { get; set; }
}
