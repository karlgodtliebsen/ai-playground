using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

/// <summary>
/// Interface for Chat Service
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Executes the chat
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> Chat(ChatMessage input, CancellationToken cancellationToken);
}
