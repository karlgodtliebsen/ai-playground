public class ViewModel
{
    public ViewState ViewState { get; set; } = new();

    public ApiKeyViewModel ApiKey { get; set; } = new();
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
}