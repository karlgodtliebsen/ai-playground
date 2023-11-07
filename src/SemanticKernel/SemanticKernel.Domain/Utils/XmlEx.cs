using System.Xml;
using System.Xml.XPath;

namespace SemanticKernel.Domain.Utils;

public static class XmlEx
{
    public static bool HasChildElements(this XmlNode elt)
    {
        if (!elt.HasChildNodes)
        {
            return false;
        }

        var childNodes = elt.ChildNodes;
        for (int i = 0, count = childNodes.Count; i < count; ++i)
        {
            if (childNodes[i]?.NodeType == XmlNodeType.Element)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Walks the Markup DOM using an XPathNavigator, allowing recursive descent WITHOUT requiring a Stack Hit
    ///     This is safe for very large and highly nested documents.
    /// </summary>
    public static IEnumerable<XmlNodeInfo> EnumerateNodes(this XmlNode node, int maxStackDepth = 32)
    {
        var nav = node.CreateNavigator();
        return nav!.EnumerateNodes(maxStackDepth);
    }

    public static IEnumerable<XmlNodeInfo> EnumerateNodes(this XmlDocument doc, int maxStackDepth = 32)
    {
        var nav = doc.CreateNavigator();
        nav!.MoveToRoot();
        return nav.EnumerateNodes(maxStackDepth);
    }

    public static IEnumerable<XmlNodeInfo> EnumerateNodes(this XPathNavigator nav, int maxStackDepth = 32)
    {
        var info = new XmlNodeInfo
        {
            StackDepth = 0
        };
        var hasChildren = nav.HasChildren;
        while (true)
        {
            info.Parent = (XmlNode)nav.UnderlyingObject!;
            if (hasChildren && info.StackDepth < maxStackDepth)
            {
                nav.MoveToFirstChild();
                info.StackDepth++;
            }
            else
            {
                var hasParent = false;
                while (hasParent = nav.MoveToParent())
                {
                    info.StackDepth--;
                    if (info.StackDepth == 0)
                    {
                        hasParent = false;
                        break;
                    }

                    if (nav.MoveToNext())
                    {
                        break;
                    }
                }

                if (!hasParent)
                {
                    break;
                }
            }

            do
            {
                info.Node = (XmlNode)nav.UnderlyingObject!;
                yield return info;
                if (hasChildren = nav.HasChildren)
                {
                    break;
                }
            } while (nav.MoveToNext());
        }
    }
}
