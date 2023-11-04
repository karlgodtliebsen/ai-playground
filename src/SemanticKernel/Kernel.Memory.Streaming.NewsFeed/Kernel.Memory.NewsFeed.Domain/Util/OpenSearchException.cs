namespace Kernel.Memory.NewsFeed.Domain.Util;

[Serializable]
public class OpenSearchException : Exception
{
    public OpenSearchException(string message) : base(message)
    {
    }

    public OpenSearchException(string message, string subTest) : base(message + "-" + subTest)
    {
    }
}
