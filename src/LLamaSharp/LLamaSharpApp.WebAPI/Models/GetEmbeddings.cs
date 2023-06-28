namespace LLamaSharpApp.WebAPI.Models;

public class GetEmbeddings
{
    public GetEmbeddings(string text)
    {
        Text = text;
    }

    public string Text { get; }
}