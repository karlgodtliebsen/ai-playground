namespace OpenAI.Client.OpenAI.Models.ChatCompletion;

public class CompletionsLogProbability
{
    /// <summary>
    /// Log Probability of Tokens.
    /// </summary>
    public IReadOnlyList<float?> TokenLogProbability { get; }
    /// <summary>
    /// Top Log Probabilities.
    /// </summary>
    public IReadOnlyList<IDictionary<string, float>> TopLogProbability { get; }
}
