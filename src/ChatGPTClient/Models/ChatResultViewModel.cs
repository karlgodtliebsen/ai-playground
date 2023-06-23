using System.Collections.ObjectModel;

namespace ChatGPTClient.Models;

public sealed class ChatResultViewModel
{
    public ObservableCollection<RichResultViewModel> Reply { get; } = new() { };
}
