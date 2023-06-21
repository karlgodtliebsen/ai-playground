namespace ChatGPTClient.Models;

public sealed class ViewModel
{
    public ViewState ViewState { get; set; } = default!;

    public ApiKeyViewModel ApiKey { get; init; } = new();
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
}
