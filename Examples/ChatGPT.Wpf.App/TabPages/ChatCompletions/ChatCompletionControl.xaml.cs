using ChatGPTClient.Models;

using Microsoft.Extensions.Options;

using OneOf;

using OpenAI.Client.Configuration;
using OpenAI.Client.Domain;
using OpenAI.Client.OpenAI.HttpClients;
using OpenAI.Client.OpenAI.Models.ChatCompletion;
using OpenAI.Client.OpenAI.Models.Requests;
using OpenAI.Client.OpenAI.Models.Responses;

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
        var prompt = viewModel!.Prompt!.Text!.Trim();

        var payload = CreatePayload(prompt);
        UpdateUIStatus(prompt);
        if (payload.Stream.HasValue && !payload.Stream.Value)
        {
            var response = await aiClient.GetChatCompletionsAsync(payload, CancellationToken.None);
            ProcessAnswer(response);
        }
        else
        {
            var responseCollection = aiClient.GetChatCompletionsStreamAsync(payload, CancellationToken.None);
            await ProcessStreamedAnswer(responseCollection);
        }
        viewModel.IsReady = true;
    }

    private void ProcessAnswer(OneOf<ChatCompletions, ErrorResponse> response)
    {
        response.Switch(
            completions =>
            {
                foreach (var choice in completions.Choices)
                {
                    viewModel.Result.Reply.Add(new RichResultViewModel()
                    {
                        Text = choice!.Message!.Content!.Trim(),
                        Kind = "A",
                        Role = "assistant",
                        Success = true,
                    });

                    messages.Add(new ChatCompletionMessage { Role = "assistant", Content = choice.Message.Content.Trim() });
                }
            },
            error =>
            {
                viewModel.Result.Reply.Add(new RichResultViewModel()
                {
                    Text = error.Error,
                    Role = "error",
                    Kind = "F",
                    Success = false,
                });
            }
        );
    }

    private async Task ProcessStreamedAnswer(IAsyncEnumerable<OneOf<ChatCompletions, ErrorResponse>> responseCollection)
    {
        await foreach (var response in responseCollection)
        {
            response.Switch(
                completions =>
                {
                    foreach (var choice in completions.Choices)
                    {
                        if (choice.Delta!.Content != null)
                        {
                            var content = choice!.Delta!.Content!.Trim();
                            viewModel.Result.Reply.Add(new RichResultViewModel()
                            {
                                Text = content,
                                Kind = "A",
                                Role = "assistant",
                                Success = true,
                            });

                            messages.Add(new ChatCompletionMessage { Role = "assistant", Content = content });
                        }
                    }
                },
                error =>
                {
                    viewModel.Result.Reply.Add(new RichResultViewModel()
                    {
                        Text = error.Error,
                        Role = "error",
                        Kind = "F",
                        Success = false,
                    });
                }
            );
        }
    }

    private ChatCompletionRequest CreatePayload(string prompt)
    {
        var selectedModel = this.viewState.SelectedModel!;
        messages.Add(new ChatCompletionMessage { Role = "user", Content = prompt });

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
        return payload;
    }

    private void UpdateUIStatus(string prompt)
    {
        viewModel.IsReady = false;
        viewModel.Result.Reply.Add(new RichResultViewModel()
        {
            Text = prompt,
            Kind = "Q",
            Role = "user",
            Success = true,
        });
    }


    private void Copy_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var result = button?.Tag as string;
        Clipboard.SetText(result);
    }
    private void Clear_OnClick(object sender, RoutedEventArgs e)
    {
        viewModel.Result.Reply.Clear();
    }
}
