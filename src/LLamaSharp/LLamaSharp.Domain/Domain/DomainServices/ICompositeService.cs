using AI.Library.HttpUtils;

using LLamaSharp.Domain.Domain.Models;

using OneOf;

namespace LLamaSharp.Domain.Domain.DomainServices;

/// <summary>
/// Interface for LLama.Examples.NewVersion Examples
/// </summary>
public interface ICompositeService
{
    /// <summary>
    /// Executes the Instructions based chat
    /// Matches LLama.Examples.NewVersion.ChatSessionStripRoleName
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionStripRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<string, ErrorResponse>> ChatSessionWithInstructionsExecutorAndNoRoleNames(ExecutorInferMessage input, CancellationToken cancellationToken);

    /// <summary>
    /// Executes the Instructions based chat
    /// Matches LLama.Examples.NewVersion.ChatSessionStripRoleName
    /// <a href="https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/NewVersion/ChatSessionWithRoleName.cs" >LLama.Examples</a>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OneOf<string, ErrorResponse>> ChatSessionWithInstructionsExecutorAndRoleNames(ExecutorInferMessage input, CancellationToken cancellationToken);

}
