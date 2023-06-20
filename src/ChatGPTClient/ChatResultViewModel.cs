using System.Collections.ObjectModel;

public class ChatResultViewModel
{
    public ObservableCollection<string> Text { get; } = new() { };
}