namespace LLamaSharp.Domain.Domain.Services;

/// <summary>
/// Keyword Text Output Stream Transform
/// </summary>
public class KeywordTextOutputStreamTransform
{
    public KeywordTextOutputStreamTransform(IEnumerable<string> keywords, int redundancyLength = 3, bool removeAllMatchedTokens = false)
    {
        Keywords = keywords;
        RedundancyLength = redundancyLength;
        RemoveAllMatchedTokens = removeAllMatchedTokens;
    }

    public IEnumerable<string> Keywords { get; set; }

    public int RedundancyLength { get; init; }
    public bool RemoveAllMatchedTokens { get; init; }

}
