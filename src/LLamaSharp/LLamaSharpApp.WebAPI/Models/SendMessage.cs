namespace LLamaSharpApp.WebAPI.Models;

public class SendMessage
{
    public SendMessage(string text)
    {
        Text = text;
    }

    public string Text { get; }
}