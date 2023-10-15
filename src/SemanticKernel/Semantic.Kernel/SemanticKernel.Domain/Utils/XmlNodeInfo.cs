using System.Xml;

namespace SemanticKernel.Tests.Utils;

public struct XmlNodeInfo
{
    public int StackDepth { get; set; }
    public XmlNode Parent { get; set; }
    public XmlNode Node { get; set; }

    public static implicit operator XmlNode(XmlNodeInfo info)
    {
        return info.Node;
    }
}
