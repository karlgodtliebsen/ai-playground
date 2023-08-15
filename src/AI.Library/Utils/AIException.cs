namespace AI.Library.Utils;

[Serializable]
public class AIException : Exception
{
    public AIException(string message) : base(message)
    {
    }

    public AIException(string message, string subTest) : base(message + "-" + subTest)
    {
    }
}
