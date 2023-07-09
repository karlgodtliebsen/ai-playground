namespace AI.CaaP.Domain;

/// <summary>
/// Chop large content into chunks
/// </summary>
public interface ITextChopperService
{
    IList<string> Chop(string content);
}
