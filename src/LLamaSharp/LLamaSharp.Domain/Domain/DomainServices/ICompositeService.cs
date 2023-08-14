using AI.Library.HttpUtils;

using LLamaSharp.Domain.Domain.Models;

using OneOf;

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
    Task<OneOf<string, ErrorResponse>> ChatSessionWithInstructionsExecutorAndRoleName(ExecutorInferMessage input, CancellationToken cancellationToken);

}
