using System.Windows;

namespace ChatGPT.Wpf.App.Models;

public sealed class RichResultViewModel
{
    public string Text { get; set; } = default!;
    public string Kind { get; set; } = default!;

    public string Role { get; set; } = default!;

    public bool Success { get; set; } = false;

    public Visibility Visibility => Kind == "A" ? Visibility.Visible : Visibility.Collapsed;
}
