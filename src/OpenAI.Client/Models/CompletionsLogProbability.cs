namespace OpenAI.Client.Models;

public class CompletionsLogProbability
{
    /// <summary> Log Probability of Tokens. </summary>
    public IReadOnlyList<float?> TokenLogProbability { get; }
    /// <summary> Top Log Probabilities. </summary>
    public IReadOnlyList<IDictionary<string, float>> TopLogProbability { get; }
}