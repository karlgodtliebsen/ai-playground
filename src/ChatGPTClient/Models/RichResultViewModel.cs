using System.Windows;

namespace ChatGPTClient.Models;

public sealed class RichResultViewModel
{
    public string Text { get; set; }
    public string Kind { get; set; }

    public string Role { get; set; }

    public bool Success { get; set; }

    public Visibility Visibility
    {
        get { return Kind == "A" ? Visibility.Visible : Visibility.Collapsed; }

    }
}
