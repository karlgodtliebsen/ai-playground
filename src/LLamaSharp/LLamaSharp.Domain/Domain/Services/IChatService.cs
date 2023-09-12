using LLamaSharp.Domain.Domain.Models;

namespace LLamaSharp.Domain.Domain.Services;

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


    /// <summary>
    /// Chat Stream
    /// </summary>
    /// <param name="chatMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> ChatStream(ChatMessage chatMessage, CancellationToken cancellationToken);
}
