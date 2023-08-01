using LLamaSharpApp.WebAPI.Domain.Models;

namespace LLamaSharpApp.WebAPI.Domain.Services;

/// <summary>
/// Interface for Chat Service
/// </summary>
public interface IChatDomainService
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
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<string> ChatStream(ChatMessage input, CancellationToken cancellationToken);
}
