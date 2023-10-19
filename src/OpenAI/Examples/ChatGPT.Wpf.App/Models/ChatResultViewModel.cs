using System.Collections.ObjectModel;

namespace ChatGPT.Wpf.App.Models;

public sealed class ChatResultViewModel
{
    public ObservableCollection<RichResultViewModel> Reply { get; } = new() { };
}
