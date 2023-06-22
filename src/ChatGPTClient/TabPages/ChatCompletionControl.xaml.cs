using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ChatGPTClient.Models;

using Microsoft.Extensions.Options;

using OpenAI.Client.AIClients;
using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;

using Model = ChatGPTClient.Models.Model;

namespace ChatGPTClient;

/// <summary>
/// Interaction logic for ChatGPTControl.xaml
/// </summary>
public partial class ChatCompletionControl : UserControl
{
    public ChatCompletionControl()
    {
        InitializeComponent();
    }


    private readonly IChatCompletionAIClient aiClient;
    private readonly IModelRequestFactory requestFactory;

    private readonly ChatCompletionViewModel ViewModel = new();
    private readonly ViewState viewState;
    private IList<ChatCompletionMessage> messages = new List<ChatCompletionMessage>();
    private ObservableCollection<Model> models;

    public ChatCompletionControl(
        IChatCompletionAIClient chatCompletionClient,
        IModelRequestFactory requestFactory,
        IOptions<OpenAIOptions> options,
        ViewState viewState
    )
    {
        this.viewState = viewState;
        this.aiClient = chatCompletionClient;
        this.requestFactory = requestFactory;
        InitializeComponent();
        ViewModel.PopupViewModel.ApiKey = options.Value.ApiKey;
        ViewModel.ViewState = viewState;
        DataContext = ViewModel;
        messages.Add(new ChatCompletionMessage { Role = "system", Content = "You are a helpful assistant that provides information." });
        ViewModel.Prompt.Text = "What is the population of the Philippines?";
        SetModels();
    }


    private void SetModels()
    {
        models = new ObservableCollection<Models.Model>();
        foreach (var model in requestFactory.GetModels("chat/completions"))
        {
            models.Add(new Model() { ModelId = model.Trim() });
        }
        this.viewState.Models = models;
        this.viewState.SelectedModel = models.FirstOrDefault();
    }

    private async void Key_OnUp(object sender, RoutedEventArgs e)
    {
        var ev = e as KeyEventArgs;
        if (ev?.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
        {
            await Submit();
        }
    }


    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        await Submit();
    }

    private async Task Submit()
    {
        if (!models.Any() || this.viewState.SelectedModel is null)
        {
            this.viewState.SelectedModel = models.First();
        }
        var selectedModel = this.viewState.SelectedModel;
        var prompt = ViewModel!.Prompt!.Text!.Trim();
        messages.Add(new ChatCompletionMessage { Role = "user", Content = prompt });
        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = selectedModel.ModelId,
                Messages = messages.ToArray(),
                MaxTokens = 100,
                Temperature = 0.0f,
                TopP = 1.0f,
                NumChoicesPerPrompt = 1,
            });

        ViewModel.Result.Reply.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Success = true,
        });

        var response = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
        if (response!.Success)
        {
            var completions = response.Value;
            foreach (var choice in completions.Choices)
            {

                ViewModel.Result.Reply.Add(new RichResultViewModel()
                {
                    Text = choice.Message.Content.Trim(),
                    Kind = "A",
                    Success = true,
                });

                messages.Add(new ChatCompletionMessage { Role = "assistant", Content = choice.Message.Content.Trim() });
            }
        }
        else
        {
            ViewModel.Result.Reply.Add(new RichResultViewModel()
            {
                Text = "Error",
                Kind = "F",
                Success = false,
            });
        }
    }


}


public class ChatCompletionPopupViewModel
{
    public string ApiKey { get; set; }
}
public class ChatCompletionViewModel
{

    public ChatCompletionPopupViewModel PopupViewModel { get; } = new ChatCompletionPopupViewModel();
    public ChatResultViewModel Result { get; set; } = new();
    public PromptTextModel Prompt { get; set; } = new();
    public ViewState ViewState { get; set; }
}
