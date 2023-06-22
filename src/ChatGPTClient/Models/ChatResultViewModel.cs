using System.Collections.ObjectModel;

namespace ChatGPTClient.Models;

public sealed class ChatResultViewModel
{
    public ObservableCollection<RichResultViewModel> Reply { get; } = new() { };
}



public sealed class RichResultViewModel
{
    public string Text { get; set; }
    public string Kind { get; set; }

    public string Role { get; set; }

    public bool Success { get; set; }

}

