using System.Collections.ObjectModel;

namespace ChatGPTClient.Models;

public sealed class ChatResultViewModel
{
    public ObservableCollection<string> Text { get; } = new() { };
}
