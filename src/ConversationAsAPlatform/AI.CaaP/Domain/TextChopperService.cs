using System.Text.RegularExpressions;

using AI.CaaP.Configuration;

using Microsoft.Extensions.Options;

namespace AI.CaaP.Domain;

public class TextChopperService : ITextChopperService
{
    private readonly ChunkOptions options;

    public TextChopperService(IOptions<ChunkOptions> options)
    {
        this.options = options.Value;
    }

    public IList<string> Chop(string content)
    {
        content = Regex.Replace(content, @"\.{2,}", " ");
        content = Regex.Replace(content, @"_{2,}", " ");
        return options.SplitByWord ? ChopByWord(content) : ChopByChar(content);
    }

    private List<string> ChopByWord(string content)
    {
        var chunks = new List<string>();

        var words = content.Split(' ')
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        var chunk = "";
        for (var i = 0; i < words.Count; i++)
        {
            chunk += words[i] + " ";
            if (chunk.Length > options.Size)
            {
                chunks.Add(chunk.Trim());
                chunk = "";
                i -= options.Conjunction;
            }
        }

        return chunks;
    }

    private List<string> ChopByChar(string content)
    {
        var chunks = new List<string>();
        var currentPos = 0;
        while (currentPos < content.Length)
        {
            var len = content.Length - currentPos > options.Size ?
                options.Size :
                content.Length - currentPos;
            var chunk = content.Substring(currentPos, len);
            chunks.Add(chunk);
            // move backward
            currentPos += options.Size - options.Conjunction;
        }
        return chunks;
    }
}
