using System.Xml;

namespace SemanticKernel.Domain.Utils;

public class XmlMarkup
{
    public XmlMarkup(string response, string? wrapperTag = null)
    {
        if (!string.IsNullOrEmpty(wrapperTag))
        {
            response = $"<{wrapperTag}>{response}</{wrapperTag}>";
        }

        Document = new XmlDocument();
        Document.LoadXml(response);
    }

    public XmlDocument Document { get; }

    public XmlNodeList SelectAllElements()
    {
        return Document.SelectNodes("//*")!;
    }

    public XmlNodeList SelectElements()
    {
        return Document.SelectNodes("/*")!;
    }
}
