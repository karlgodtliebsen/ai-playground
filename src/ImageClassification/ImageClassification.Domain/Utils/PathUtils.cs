namespace ImageClassification.Domain.Utils;

public static class PathUtils
{
    private const string Pattern = "{imageSetPath}";
    public static string GetPath(string filePath, string imageSet, string? subPath = null)
    {
        var p = filePath.Replace(Pattern, imageSet);
        if (subPath is null)
        {
            return Path.GetFullPath(p);
        }
        return Path.GetFullPath(Path.Combine(p, subPath));
    }
}
