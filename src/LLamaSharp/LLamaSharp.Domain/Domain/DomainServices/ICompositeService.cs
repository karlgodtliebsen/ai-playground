using LLamaSharp.Domain.Domain.Models;

namespace LLamaSharp.Domain.Domain.DomainServices;

/// <summary>
/// Interface for Chat Service
/// </summary>
public interface ICompositeService
{
    /// <summary>
    /// Executes the chat
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> ChatUsingInstructionsSessionWithRoleName(ExecutorInferMessage input, CancellationToken cancellationToken);

}
