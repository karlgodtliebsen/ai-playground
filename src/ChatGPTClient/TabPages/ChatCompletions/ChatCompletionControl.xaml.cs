using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

    private readonly ChatCompletionViewModel viewModel = new();
    private readonly ViewState viewState;
    private readonly IList<ChatCompletionMessage> messages = new List<ChatCompletionMessage>();
    private readonly ObservableCollection<Model> models = new();

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
        messages.Add(new ChatCompletionMessage { Role = "system", Content = "You are a helpful assistant that provides information." });

        InitializeComponent();
        viewModel.ViewState = viewState;
        DataContext = viewModel;
        viewModel.Prompt.Text = "How many planets are there in our Solar system?";
    }


    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        SetOpenAIModels();
    }

    private void SetOpenAIModels()
    {
        models.Clear();
        foreach (var model in requestFactory.GetModels("chat/completions"))
        {
            models.Add(new Model() { ModelId = model.Trim() });
        }
        this.viewState.Models = models;
        this.viewState.SelectedModel = models.FirstOrDefault(m => m.ModelId.Contains("-turbo"));
    }

    private async void Key_OnUp(object sender, KeyEventArgs ev)
    {
        if (ev?.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
        {
            await Submit();
        }
    }

    private async void ButtonBase_OnClick(object sender, EventArgs e)
    {
        await Submit();
    }

    private async Task Submit()
    {
        var selectedModel = this.viewState.SelectedModel;
        var prompt = viewModel!.Prompt!.Text!.Trim();
        messages.Add(new ChatCompletionMessage { Role = "user", Content = prompt });
        viewModel.Result.Reply.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Role = "user",
            Success = true,
        });

        var options = viewModel.Options;
        var payload = requestFactory.CreateRequest<ChatCompletionRequest>(() =>
            new ChatCompletionRequest
            {
                Model = selectedModel.ModelId,
                Messages = messages.ToArray(),
                MaxTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                NumChoicesPerPrompt = options.NumChoicesPerPrompt,
                Stream = options.Stream,
                Logprobs = options.Logprobs,
                Stop = options.Stop,
            });
        viewModel.IsReady = false;

        var response = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
        if (response!.Success)
        {
            var completions = response.Value;
            foreach (var choice in completions.Choices)
            {

                viewModel.Result.Reply.Add(new RichResultViewModel()
                {
                    Text = choice.Message.Content.Trim(),
                    Kind = "A",
                    Role = "assistant",
                    Success = true,
                });

                messages.Add(new ChatCompletionMessage { Role = "assistant", Content = choice.Message.Content.Trim() });
            }
        }
        else
        {
            viewModel.Result.Reply.Add(new RichResultViewModel()
            {
                Text = "Error",
                Kind = "F",
                Success = false,
            });
        }
        viewModel.IsReady = true;

    }


    private void Copy_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var result = button?.Tag as string;
        Clipboard.SetText(result);
    }

}
